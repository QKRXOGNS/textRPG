﻿
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
    public int Health { get; set; }
    public int AttackPower { get; set; }
    public int Defense { get; set; }
    public int Gold { get; set; }

    // 장착 슬롯
    public IEquipment EquippedWeapon { get; private set; }
    public IEquipment EquippedArmor { get; private set; }

    public bool IsDead => Health <= 0;

    // 장비 보너스를 반영한 공격력과 방어력
    public int Attack => new Random().Next(10, AttackPower) + (EquippedWeapon?.AttackBonus ?? 0);
    public int TotalDefense => Defense + (EquippedArmor?.DefenseBonus ?? 0); // 방어력 적용

    public Warrior(string name)
    {
        Name = name;
        Health = 100;
        AttackPower = 20;
        Defense = 5;
        Gold = 100;
    }

    public bool TakeDamage(int damage)
    {
        if (IsDead) return true; // 이미 죽었으면 추가 피해 없음

        int actualDamage = Math.Max(0, damage - TotalDefense);
        Health -= actualDamage;
        Console.WriteLine($"{Name}이(가) {actualDamage}의 데미지를 받았습니다. (방어력 {TotalDefense} 적용)");

        if (Health <= 0) // 체력이 0 이하가 되면
        {
            Console.WriteLine($"{Name}이(가) 쓰러졌습니다. 마을로 돌아갑니다...");
            Gold = (int)(Gold * 0.8); // 골드 20% 차감
            Health = 100; // 체력 완전 회복
            Console.WriteLine($"마을에서 회복되었습니다. 체력: 100, 남은 골드: {Gold}");
            return true; // 사망 여부 반환 (전투 종료)
        }

        return false; // 생존 상태 반환
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
                Console.WriteLine("5. 게임 종료");
                Console.WriteLine("6. 저장하기");
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
                        Console.WriteLine("게임을 종료합니다.");
                        return;
                    case "6":
                        SaveSystem.SaveGame(player, inventory);
                        break;
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
            Console.WriteLine($"체력: {player.Health}/100");

            // 장비 효과 반영하여 출력
            int totalAttack = player.AttackPower + (player.EquippedWeapon?.AttackBonus ?? 0);
            int totalDefense = player.Defense + (player.EquippedArmor?.DefenseBonus ?? 0);

            Console.WriteLine($"공격력: {totalAttack} (기본: {player.AttackPower}, 장비 추가: {player.EquippedWeapon?.AttackBonus ?? 0})");
            Console.WriteLine($"방어력: {totalDefense} (기본: {player.Defense}, 장비 추가: {player.EquippedArmor?.DefenseBonus ?? 0})");
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
                if (inventory[itemIndex - 1] is IEquipment equipment)
                {
                    player.EquipItem(equipment);
                }
                else
                {
                    inventory[itemIndex - 1].Use(player);
                    inventory.RemoveAt(itemIndex - 1);
                }
            }
        }

        static void OpenShop(Warrior player, List<IItem> inventory, HashSet<string> purchasedEquipment)
        {
            Console.WriteLine("\n==== [상점] ====");

            // 상점에서 판매할 아이템 목록
            List<IItem> shopItems = new List<IItem>
    {
        new HealthPotion(), new StrengthPotion(), new DefensePotion(),
        new Weapon("전사의 검", 100, 15),
        new Armor("강철 갑옷", 150, 20, 10)
    };

            // 상점 아이템 목록 출력
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

                // 장비 아이템인지 확인하고 이미 구매했다면 경고 메시지 출력
                if (item is IEquipment && purchasedEquipment.Contains(item.Name))
                {
                    Console.WriteLine("이미 구매한 아이템입니다.");
                    return;
                }

                if (player.Gold >= item.Price)
                {
                    player.Gold -= item.Price;
                    inventory.Add(item);

                    // 장비 아이템은 구매 제한
                    if (item is IEquipment)
                    {
                        purchasedEquipment.Add(item.Name);
                    }

                    Console.WriteLine($"{item.Name}을 구매했습니다!");
                }
                else
                {
                    Console.WriteLine("골드가 부족합니다.");
                }
            }
        }

        static void EnterDungeon(Warrior player, List<IItem> inventory)
        {
            Console.WriteLine("\n1. 고블린 던전 \n2. 드래곤 던전");
            Monster monster = Console.ReadLine() == "1" ? new Goblin() : new Dragon();
            new Stage(player, monster, new List<IItem>()).Start();
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
            writer.WriteLine(player.Health);
            writer.WriteLine(player.AttackPower);
            writer.WriteLine(player.Defense);
            writer.WriteLine(player.Gold);

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
            int health = int.Parse(reader.ReadLine());
            int attackPower = int.Parse(reader.ReadLine());
            int defense = int.Parse(reader.ReadLine());
            int gold = int.Parse(reader.ReadLine());

            player = new Warrior(name)
            {
                Health = health,
                AttackPower = attackPower,
                Defense = defense,
                Gold = gold
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

