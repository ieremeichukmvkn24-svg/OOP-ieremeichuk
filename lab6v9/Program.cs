using System;
using System.Collections.Generic;

namespace Lab6Variant9
{
    /// <summary>
    /// Базовий клас, що представляє зброю.
    /// </summary>
    public class Weapon
    {
        private string _name;
        private int _damage;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва зброї не може бути порожньою.", nameof(value));
                _name = value;
            }
        }

        public int Damage
        {
            get => _damage;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Ушкодження (Damage) не може бути від'ємним.", nameof(value));
                _damage = value;
            }
        }

        public Weapon(string name, int damage)
        {
            Name = name;
            Damage = damage;
        }

        /// <summary>
        /// Віртуальний метод для атаки, який буде перевизначено у похідних класах.
        /// </summary>
        public virtual void Attack()
        {
            Console.WriteLine($"[Weapon] Зброя '{Name}' завдає базового удару з шкодою {Damage}.");
        }

        /// <summary>
        /// Звичайний (невіртуальний) метод для отримання типу зброї.
        /// </summary>
        public string GetWeaponType()
        {
            return "General Weapon";
        }
    }

    /// <summary>
    /// Похідний клас, що представляє меч.
    /// </summary>
    public class Sword : Weapon
    {
        public string Material { get; set; }

        public Sword(string name, int damage, string material) 
            : base(name, damage)
        {
            Material = string.IsNullOrWhiteSpace(material) ? "Steel" : material;
        }

        /// <summary>
        /// Перевизначення (override) віртуального методу Attack.
        /// </summary>
        public override void Attack()
        {
            Console.WriteLine($"[Sword] Меч '{Name}' з матеріалу '{Material}' робить рубаючий випад і завдає {Damage + 15} кричного ушкодження!");
        }

        /// <summary>
        /// Унікальний метод класу Sword.
        /// </summary>
        public void Parry()
        {
            Console.WriteLine($"[Sword] Меч '{Name}' успішно парирує ворожу атаку!");
        }

        /// <summary>
        /// Приховування (new) невіртуального методу базового класу.
        /// </summary>
        public new string GetWeaponType()
        {
            return "Melee Weapon (Sword)";
        }
    }

    /// <summary>
    /// Похідний клас, що представляє лук.
    /// </summary>
    public class Bow : Weapon
    {
        public string ArrowType { get; set; }

        public Bow(string name, int damage, string arrowType) 
            : base(name, damage)
        {
            ArrowType = string.IsNullOrWhiteSpace(arrowType) ? "Standard" : arrowType;
        }

        /// <summary>
        /// Перевизначення (override) віртуального методу Attack.
        /// </summary>
        public override void Attack()
        {
            Console.WriteLine($"[Bow] Лук '{Name}' вистрілює стрілу типу '{ArrowType}', завдаючи {Damage} дальнобійного ушкодження!");
        }

        /// <summary>
        /// Унікальний метод класу Bow.
        /// </summary>
        public void Aim()
        {
            Console.WriteLine($"[Bow] Лук '{Name}' прицілюється у вразливу точку противника.");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ ЛАБОРАТОРНОЇ РОБОТИ №6 (ВАРАНТ 9) ===\n");

            // 1. Створення об'єктів
            Console.WriteLine("--- 1. Створення об'єктів ---");
            Weapon baseWeapon = new Weapon("Звичайний дубець", 10);
            Sword sword = new Sword("Екскалібур", 45, "Дамаська сталь");
            Bow bow = new Bow("Соколине око", 35, "Огненна стріла");

            Console.WriteLine($"Створено базову зброю: {baseWeapon.Name}");
            Console.WriteLine($"Створено меч: {sword.Name} (Матеріал: {sword.Material})");
            Console.WriteLine($"Створено лук: {bow.Name} (Тип стріл: {bow.ArrowType})\n");

            // 2. Демонстрація унікальних методів
            Console.WriteLine("--- 2. Викликаємо унікальні методи похідних класів ---");
            sword.Parry();
            bow.Aim();
            Console.WriteLine();

            // 3. Демонстрація поліморфізму (virtual / override)
            Console.WriteLine("--- 3. Демонстрація поліморфізму (virtual / override) ---");
            List<Weapon> armory = new List<Weapon> { baseWeapon, sword, bow };

            foreach (var weapon in armory)
            {
                // Для кожного об'єкта викликається саме СВОЯ реалізація Attack(),
                // незважаючи на те, що тип змінної списку — Weapon.
                weapon.Attack();
            }
            Console.WriteLine();

            // 4. Демонстрація різниці між override та new
            Console.WriteLine("--- 4. Демонстрація різниці між override та new ---");
            
            // Створюємо екземпляр Sword
            Sword directSword = new Sword("Кладенець", 50, "Титан");
            
            // Вказівник типу базового класу Weapon, що вказує на той самий об'єкт Sword
            Weapon baseRefToSword = directSword;

            Console.WriteLine("A) Виклик перевизначеного віртуального методу Attack():");
            Console.Write("  • Через посилання типу Sword: ");
            directSword.Attack();
            Console.Write("  • Через посилання типу Weapon: ");
            baseRefToSword.Attack(); // Працює поліморфізм: викликається Sword.Attack()

            Console.WriteLine("\nB) Виклик прихованого через 'new' методу GetWeaponType():");
            Console.WriteLine($"  • Через посилання типу Sword (directSword.GetWeaponType()):  {directSword.GetWeaponType()}");
            Console.WriteLine($"  • Через посилання типу Weapon (baseRefToSword.GetWeaponType()): {baseRefToSword.GetWeaponType()}");
            Console.WriteLine("   -> Пояснення: при 'new' метод обирається за ТИПОМ ПОСИЛАННЯ, а не за фактичним типом об'єкта!");

            Console.WriteLine("\nЗавершення роботи програми.");
        }
    }
}