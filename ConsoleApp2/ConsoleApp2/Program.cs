
// 캐릭터 인터페이스
public interface ICharacter
{
    string Name { get; }
    int Health { get; set; }
    int Attack { get; }
    int Defense { get; }
    bool IsDead { get; }
    bool TakeDamage(int damage); // 반환 타입을 bool로 변경 (사망 여부 반환)
}


// 전사 클래스
public class Warrior : ICharacter
{
    public string Name { get; }
    public int Level { get; set; } // 레벨 추가 (set을 private에서 public으로 변경)
    public int Health { get; set; }
    public int AttackPower { get; set; }
    public int Defense { get; set; }
    public int Gold { get; set; }
    public int DungeonClears { get; set; } // 던전 클리어 횟수 저장


    // 장착 슬롯
    public IEquipment EquippedWeapon { get; private set; }
    public IEquipment EquippedArmor { get; private set; }

    private int dungeonClears; // 던전 클리어 횟수 추적

    public bool IsDead => Health <= 0;

    // 장비 보너스를 반영한 공격력과 방어력
    public int Attack => new Random().Next(10, Math.Max(10, AttackPower) + 1);

    public int TotalDefense => Defense + (EquippedArmor?.DefenseBonus ?? 0);



    public Warrior(string name)
    {
        Name = name;
        Level = 1;
        Health = 100;
        AttackPower = 20;
        Defense = 5;
        Gold = 100;
        DungeonClears = 0;
    }



    public bool TakeDamage(int damage)
    {
        if (IsDead) return true; // 이미 죽었으면 추가 피해 없음

        int actualDamage = Math.Max(0, damage - TotalDefense);
        Health -= actualDamage;
        Console.WriteLine($"{Name}이(가) {actualDamage}의 데미지를 받았습니다. (방어력 {TotalDefense} 적용)");

        if (Health <= 0)
        {
            Console.WriteLine($"{Name}이(가) 쓰러졌습니다. 마을로 돌아갑니다...");
            Gold = (int)(Gold * 0.8);
            Health = 100;
            Console.WriteLine($"마을에서 회복되었습니다. 체력: 100, 남은 골드: {Gold}");
            return true;
        }

        return false;
    }
    public void ClearDungeon()
    {
        DungeonClears++;

        if (DungeonClears >= Level)
        {
            LevelUp();
            DungeonClears = 0;
        }
    }

    private void LevelUp()
    {
        Level++;
        AttackPower += (int)(0.5 * Level);
        Defense += 1;

        Console.WriteLine($"축하합니다! {Name}이(가) Lv.{Level}으로 레벨업 했습니다!");
        Console.WriteLine($"공격력 +0.5 → {AttackPower}, 방어력 +1 → {Defense}");
    }

    public void EquipItem(IEquipment equipment)
    {
        if (equipment is Weapon)
        {
            EquippedWeapon = equipment;
            Console.WriteLine($"{Name}이(가) {equipment.Name}을 장착했습니다! (공격력 +{equipment.AttackBonus})");
        }
        else if (equipment is Armor)
        {
            EquippedArmor = equipment;
            Console.WriteLine($"{Name}이(가) {equipment.Name}을 장착했습니다! (방어력 +{equipment.DefenseBonus}, 체력 +{equipment.HealthBonus})");
            Health += equipment.HealthBonus;
        }
    }
    //장비 벗기
    public void UnequipItem(IEquipment equipment)
    {
        if (equipment == EquippedWeapon)
        {
            EquippedWeapon = null;
            Console.WriteLine($"무기 {equipment.Name}을(를) 해제했습니다!");
        }
        else if (equipment == EquippedArmor)
        {
            EquippedArmor = null;
            Health -= equipment.HealthBonus; // 장비가 추가했던 체력 보너스 제거
            Console.WriteLine($"방어구 {equipment.Name}을(를) 해제했습니다!");
        }
    }

    public void Rest()
    {
        int restCost = 500;
        if (Gold >= restCost)
        {
            Gold -= restCost;
            Health = 100;
            Console.WriteLine("\n휴식을 취하여 체력을 완전히 회복했습니다!");
            Console.WriteLine($"남은 골드: {Gold}");
        }
        else
        {
            Console.WriteLine("\n보유 금액이 부족하여 휴식을 취할 수 없습니다.");
        }
    }

}

// 몬스터 클래스
public class Monster : ICharacter
{
    public string Name { get; }
    public int Health { get; set; }
    public int Attack => new Random().Next(10, 20);
    public int Defense => 0; // 몬스터는 기본적으로 방어력이 없음
    public bool IsDead => Health <= 0;
    public int GoldReward { get; } // 처치 시 보상 골드

    public Monster(string name, int health, int goldReward)
    {
        Name = name;
        Health = health;
        GoldReward = goldReward;
    }

    public bool TakeDamage(int damage)
    {
        Health -= damage;
        Console.WriteLine($"{Name}이(가) {damage}의 데미지를 받았습니다.");

        if (IsDead)
        {
            Console.WriteLine($"{Name}이(가) 쓰러졌습니다.");
            return true; // 몬스터 사망
        }

        return false; // 몬스터 생존
    }

}

// 몬스터 종류
public class Goblin : Monster
{
    public Goblin() : base("고블린", 50, 30) { }
}

public class Troll : Monster
{
    public Troll() : base("트롤", 80, 50) { } // 체력 80, 골드 보상 50
}


public class Dragon : Monster
{
    public Dragon() : base("드래곤", 100, 100) { }
}

// 아이템 인터페이스
public interface IItem
{
    string Name { get; }
    int Price { get; } // 가격 추가
    void Use(Warrior warrior);
}

// 체력 포션
public class HealthPotion : IItem
{
    public string Name => "체력 포션";
    public int Price => 20;

    public void Use(Warrior warrior)
    {
        Console.WriteLine($"{Name}을 사용했습니다. 체력이 50 회복됩니다.");
        warrior.Health = Math.Min(100, warrior.Health + 50);
    }
}

//장비 인테페이스
public interface IEquipment : IItem
{
    int HealthBonus { get; }
    int AttackBonus { get; }
    int DefenseBonus { get; }
}

// 무기 클래스
public class Weapon : IEquipment
{
    public string Name { get; }
    public int Price { get; }
    public int HealthBonus => 0;
    public int AttackBonus { get; }
    public int DefenseBonus => 0;

    public Weapon(string name, int price, int attackBonus)
    {
        Name = name;
        Price = price;
        AttackBonus = attackBonus;
    }

    public void Use(Warrior warrior)
    {
        warrior.EquipItem(this);
    }
}

// 방어구 클래스
public class Armor : IEquipment
{
    public string Name { get; }
    public int Price { get; }
    public int HealthBonus { get; }
    public int AttackBonus => 0;
    public int DefenseBonus { get; }

    public Armor(string name, int price, int healthBonus, int defenseBonus)
    {
        Name = name;
        Price = price;
        HealthBonus = healthBonus;
        DefenseBonus = defenseBonus;
    }

    public void Use(Warrior warrior)
    {
        warrior.EquipItem(this);
    }
}


// 공격력 포션
public class StrengthPotion : IItem
{
    public string Name => "공격력 포션";
    public int Price => 30;

    public void Use(Warrior warrior)
    {
        Console.WriteLine($"{Name}을 사용했습니다. 공격력이 10 증가합니다.");
        warrior.AttackPower += 10;
    }
}

// 방어력 포션
public class DefensePotion : IItem
{
    public string Name => "방어력 포션";
    public int Price => 25;

    public void Use(Warrior warrior)
    {
        Console.WriteLine($"{Name}을 사용했습니다. 방어력이 5 증가합니다.");
        warrior.Defense += 5;
    }
}

// 스테이지
public class Stage
{
    private Warrior player;
    private Monster monster;
    private List<IItem> rewards;

    public Stage(Warrior player, Monster monster, List<IItem> rewards)
    {
        this.player = player;
        this.monster = monster;
        this.rewards = rewards;
    }

    public void Start()
    {
        Console.WriteLine($"전투 시작! {player.Name} VS {monster.Name}");
        Thread.Sleep(1000);

        while (!player.IsDead && !monster.IsDead)
        {
            Console.WriteLine($"{player.Name}의 턴!");
            Thread.Sleep(1000);

            if (monster.TakeDamage(player.Attack)) // 몬스터가 죽으면 즉시 전투 종료
                break;

            Console.WriteLine($"{monster.Name}의 턴!");
            Thread.Sleep(1000);

            if (player.TakeDamage(monster.Attack)) // 플레이어가 죽으면 전투 종료
                return;
        }

        Thread.Sleep(1000);

        if (monster.IsDead)
        {
            Console.WriteLine($"{monster.Name}을(를) 처치했습니다! 골드 {monster.GoldReward} 획득!");
            player.Gold += monster.GoldReward;

            // 던전 클리어 처리
            player.ClearDungeon();
        }
    }
}



// 메인 프로그램
class Program
{
    static void Main()
    {
        Warrior player;
        List<IItem> inventory;

        // 저장 파일이 존재하는지 확인
        if (File.Exists("save.txt"))
        {
            if (SaveSystem.LoadGame(out player, out inventory))
            {
                Console.WriteLine("\n게임을 불러왔습니다!");
            }
            else
            {
                Console.WriteLine("\n저장된 데이터를 불러오는 데 실패했습니다.");
                return;
            }
        }
        else
        {
            // 저장 파일이 없으면 새 게임 시작
            Console.Write("당신의 캐릭터 이름을 입력하세요: ");
            string playerName = Console.ReadLine();
            player = new Warrior(playerName);
            inventory = new List<IItem>();
        }

        HashSet<string> purchasedEquipment = new HashSet<string>();

        while (true)
        {
            Console.WriteLine("\n==== [메뉴] ====");
            Console.WriteLine("1. 내 정보");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전");
            Console.WriteLine("5. 휴식하기");
            Console.WriteLine("6. 저장하기");
            Console.WriteLine("7. 게임 종료");
            Console.Write("선택: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowPlayerInfo(player);
                    break;
                case "2":
                    ShowInventory(player, inventory);
                    break;
                case "3":
                    OpenShop(player, inventory, purchasedEquipment);
                    break;
                case "4":
                    EnterDungeon(player, inventory);
                    break;
                case "5":
                    player.Rest();
                    break;
                case "6":
                    SaveSystem.SaveGame(player, inventory);
                    break;
                case "7":
                    Console.WriteLine("게임을 종료합니다.");
                    return;
                default:
                    Console.WriteLine("올바른 번호를 입력하세요.");
                    break;
            }
        }

    }



    static void ShowPlayerInfo(Warrior player)
    {
        Console.WriteLine("\n==== [내 정보] ====");
        Console.WriteLine($"이름: {player.Name}");
        Console.WriteLine($"레벨: {player.Level}");
        Console.WriteLine($"체력: {player.Health}/100");

        // 장비 효과 반영하여 출력
        int totalAttack = player.AttackPower + (player.EquippedWeapon?.AttackBonus ?? 0) + (int)(player.Level * 0.5);
        int totalDefense = player.Defense + (player.EquippedArmor?.DefenseBonus ?? 0) + (player.Level * 1);

        Console.WriteLine($"공격력: {totalAttack} (기본: {player.AttackPower}, 레벨 보너스: {player.Level * 0.5})");
        Console.WriteLine($"방어력: {totalDefense} (기본: {player.Defense}, 레벨 보너스: {player.Level * 1})");
        Console.WriteLine($"골드: {player.Gold}");
    }


    static void ShowInventory(Warrior player, List<IItem> inventory)
    {
        Console.WriteLine("\n==== [인벤토리] ====");
        if (inventory.Count == 0)
        {
            Console.WriteLine("인벤토리가 비어 있습니다.");
            return;
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            string equipped = "";
            if (inventory[i] is IEquipment eq)
            {
                if (eq == player.EquippedWeapon || eq == player.EquippedArmor)
                    equipped = " [E]";
            }
            Console.WriteLine($"{i + 1}. {inventory[i].Name}{equipped}");
        }

        Console.Write("사용할 아이템 번호 (취소: 0): ");
        if (int.TryParse(Console.ReadLine(), out int itemIndex) && itemIndex > 0 && itemIndex <= inventory.Count)
        {
            IItem selectedItem = inventory[itemIndex - 1];

            if (selectedItem is IEquipment equipment)
            {
                if (equipment == player.EquippedWeapon || equipment == player.EquippedArmor)
                {
                    player.UnequipItem(equipment);
                }
                else
                {
                    player.EquipItem(equipment);
                }
            }
            else
            {
                selectedItem.Use(player);
                inventory.RemoveAt(itemIndex - 1);
            }
        }
    }


    static void OpenShop(Warrior player, List<IItem> inventory, HashSet<string> purchasedEquipment)
    {
        while (true)
        {
            Console.WriteLine("\n==== [상점] ====");
            Console.WriteLine("1. 구매");
            Console.WriteLine("2. 판매");
            Console.WriteLine("3. 나가기");
            Console.Write("선택: ");
            string shopChoice = Console.ReadLine();

            switch (shopChoice)
            {
                case "1":
                    BuyItem(player, inventory, purchasedEquipment);
                    break;
                case "2":
                    SellItem(player, inventory);
                    break;
                case "3":
                    Console.WriteLine("상점을 나갑니다.");
                    return;
                default:
                    Console.WriteLine("올바른 번호를 입력하세요.");
                    break;
            }
        }
    }

    //구매기능
    static void BuyItem(Warrior player, List<IItem> inventory, HashSet<string> purchasedEquipment)
    {
        Console.WriteLine("\n==== [구매 가능 아이템] ====");
        List<IItem> shopItems = new List<IItem>
    {
        new HealthPotion(), new StrengthPotion(), new DefensePotion(),
        new Weapon("전사의 검", 100, 15),
        new Armor("강철 갑옷", 150, 20, 10)
    };

        for (int i = 0; i < shopItems.Count; i++)
        {
            string status = (shopItems[i] is IEquipment && purchasedEquipment.Contains(shopItems[i].Name))
                ? " [판매 완료]"
                : "";
            Console.WriteLine($"{i + 1}. {shopItems[i].Name} - {shopItems[i].Price}골드{status}");
        }

        Console.Write("구매할 아이템 번호 (취소: 0): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= shopItems.Count)
        {
            IItem item = shopItems[choice - 1];

            if (item is IEquipment && purchasedEquipment.Contains(item.Name))
            {
                Console.WriteLine("이미 구매한 아이템입니다.");
                return;
            }

            if (player.Gold >= item.Price)
            {
                player.Gold -= item.Price;
                inventory.Add(item);
                if (item is IEquipment)
                    purchasedEquipment.Add(item.Name);

                Console.WriteLine($"{item.Name}을 구매했습니다!");
            }
            else
            {
                Console.WriteLine("골드가 부족합니다.");
            }
        }
    }

    //판매기능 추가
    static void SellItem(Warrior player, List<IItem> inventory)
    {
        if (inventory.Count == 0)
        {
            Console.WriteLine("\n인벤토리에 판매할 아이템이 없습니다.");
            return;
        }

        while (true)
        {
            Console.WriteLine("\n==== [판매할 아이템 선택] ====");
            for (int i = 0; i < inventory.Count; i++)
            {
                int sellPrice = (int)(inventory[i].Price * 0.7); // 70% 가격
                string equipped = "";
                if (inventory[i] is IEquipment eq)
                {
                    if (eq == player.EquippedWeapon || eq == player.EquippedArmor)
                        equipped = " [E]";
                }
                Console.WriteLine($"{i + 1}. {inventory[i].Name}{equipped} - 판매 가격: {sellPrice}골드");
            }
            Console.WriteLine("0. 나가기");

            Console.Write("판매할 아이템 번호를 선택하세요: ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                if (choice == 0)
                {
                    Console.WriteLine("상점 판매를 취소합니다.");
                    return;
                }

                if (choice > 0 && choice <= inventory.Count)
                {
                    IItem itemToSell = inventory[choice - 1];

                    if (itemToSell is IEquipment equipment)
                    {
                        if (equipment == player.EquippedWeapon || equipment == player.EquippedArmor)
                        {
                            player.UnequipItem(equipment);
                        }
                    }

                    int sellPrice = (int)(itemToSell.Price * 0.7);
                    player.Gold += sellPrice;
                    inventory.RemoveAt(choice - 1);

                    Console.WriteLine($"{itemToSell.Name}을(를) 판매하여 {sellPrice}골드를 획득했습니다!");
                }
                else
                {
                    Console.WriteLine("올바른 번호를 입력하세요.");
                }
            }
            else
            {
                Console.WriteLine("숫자를 입력하세요.");
            }
        }
    }


    static void EnterDungeon(Warrior player, List<IItem> inventory)
    {
        List<Dungeon> dungeons = new List<Dungeon>
    {
        new Dungeon("고블린 동굴", 1, 1000, new Goblin()),
        new Dungeon("트롤 협곡", 3, 1700, new Troll()),
        new Dungeon("용의 둥지", 5, 2500, new Dragon())
    };

        Console.WriteLine("\n==== [던전 선택] ====");
        for (int i = 0; i < dungeons.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {dungeons[i].Name} (추천 레벨: {dungeons[i].RequiredLevel})");
        }
        Console.Write("입장할 던전을 선택하세요 (취소: 0): ");

        if (!int.TryParse(Console.ReadLine(), out int dungeonChoice) || dungeonChoice < 1 || dungeonChoice > dungeons.Count)
        {
            Console.WriteLine("🚨 올바른 던전 번호를 입력하세요.");
            return;
        }

        Dungeon selectedDungeon = dungeons[dungeonChoice - 1];

        bool isUnderleveled = player.Level < selectedDungeon.RequiredLevel;
        if (isUnderleveled)
        {
            Console.WriteLine("레벨이 부족합니다! 패널티가 적용됩니다!");
            Console.WriteLine("공격력 30% 감소, 방어력 50% 감소 상태로 입장합니다!");
        }

        Monster monster = selectedDungeon.Monster;
        if (monster == null)
        {
            Console.WriteLine("몬스터 생성에 실패했습니다.");
            return;
        }

        int originalAttack = player.AttackPower;
        int originalDefense = player.Defense;

        if (isUnderleveled)
        {
            player.AttackPower = Math.Max(10, (int)(player.AttackPower * 0.7));
            player.Defense = Math.Max(1, (int)(player.Defense * 0.5));
        }

        Console.WriteLine($" {selectedDungeon.Name}에 입장하여 {monster.Name}과(와) 전투를 시작합니다!");

        new Stage(player, monster, new List<IItem>()).Start();

        if (!player.IsDead)
        {
            player.Gold += selectedDungeon.RewardGold;
            Console.WriteLine($" 던전을 클리어했습니다! {selectedDungeon.RewardGold}골드를 획득했습니다!");
            player.ClearDungeon();
        }

        player.AttackPower = originalAttack;
        player.Defense = originalDefense;
    }





    public class Dungeon
    {
        public string Name { get; }
        public int RequiredLevel { get; }
        public int RewardGold { get; }
        public Monster Monster { get; }

        public Dungeon(string name, int requiredLevel, int rewardGold, Monster monster)
        {
            Name = name;
            RequiredLevel = requiredLevel;
            RewardGold = rewardGold;
            Monster = monster;
        }
    }



    public static class SaveSystem
    {
        private static readonly string SaveFilePath = "save.txt";

        // 게임 저장 메서드
        public static void SaveGame(Warrior player, List<IItem> inventory)
        {
            using (StreamWriter writer = new StreamWriter(SaveFilePath))
            {
                writer.WriteLine(player.Name);
                writer.WriteLine(player.Level); // 레벨 저장
                writer.WriteLine(player.Health);
                writer.WriteLine(player.AttackPower);
                writer.WriteLine(player.Defense);
                writer.WriteLine(player.Gold);
                writer.WriteLine(player.DungeonClears); // 던전 클리어 횟수 저장

                // 인벤토리 아이템 저장
                foreach (var item in inventory)
                {
                    writer.WriteLine(item.Name);
                }
            }

            Console.WriteLine("\n게임이 저장되었습니다! (save.txt)");
        }

        // 게임 불러오기 메서드
        public static bool LoadGame(out Warrior player, out List<IItem> inventory)
        {
            inventory = new List<IItem>();

            if (!File.Exists(SaveFilePath))
            {
                player = null;
                return false;
            }

            using (StreamReader reader = new StreamReader(SaveFilePath))
            {
                string name = reader.ReadLine();
                if (string.IsNullOrEmpty(name)) // Null 값 확인
                {
                    Console.WriteLine(" 저장된 데이터가 잘못되었습니다.");
                    player = null;
                    return false;
                }

                int level = int.Parse(reader.ReadLine() ?? "1");
                int health = int.Parse(reader.ReadLine() ?? "100");
                int attackPower = int.Parse(reader.ReadLine() ?? "20");
                int defense = int.Parse(reader.ReadLine() ?? "5");
                int gold = int.Parse(reader.ReadLine() ?? "100");
                int dungeonClears = int.Parse(reader.ReadLine() ?? "0");

                player = new Warrior(name)
                {
                    Level = level,
                    Health = health,
                    AttackPower = attackPower,
                    Defense = defense,
                    Gold = gold,
                    DungeonClears = dungeonClears
                };

                // 인벤토리 불러오기
                string itemName;
                while ((itemName = reader.ReadLine()) != null)
                {
                    IItem item = CreateItemByName(itemName);
                    if (item != null)
                        inventory.Add(item);
                }
            }

            Console.WriteLine("\n저장된 게임을 불러왔습니다!");
            return true;
        }


        // 아이템 이름을 기반으로 객체를 생성하는 메서드
        private static IItem CreateItemByName(string name)
        {
            return name switch
            {
                "체력 포션" => new HealthPotion(),
                "공격력 포션" => new StrengthPotion(),
                "방어력 포션" => new DefensePotion(),
                "전사의 검" => new Weapon("전사의 검", 100, 15),
                "강철 갑옷" => new Armor("강철 갑옷", 150, 20, 10),
                _ => null
            };
        }

    }
}

