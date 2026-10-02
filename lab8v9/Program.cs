using System;
using System.Collections.Generic;
using System.Text;

namespace Lab8Variant9
{
    /// <summary>
    /// Базовий клас, що представляє загальний інструмент.
    /// </summary>
    public class Tool
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва інструмента не може бути порожньою.", nameof(value));
                _name = value;
            }
        }

        public Tool(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Віртуальний метод для використання інструмента.
        /// Повертає рядковий опис виконаної дії.
        /// </summary>
        public virtual string Use()
        {
            return $"[Tool] Інструмент '{Name}' застосовано за загальним призначенням.";
        }
    }

    /// <summary>
    /// Похідний клас: Молоток.
    /// </summary>
    public class Hammer : Tool
    {
        public double Weight { get; set; } // Вага в кг

        public Hammer(string name, double weight) : base(name)
        {
            Weight = weight > 0 ? weight : 0.5;
        }

        /// <summary>
        /// Перевизначення (override) віртуального методу Use.
        /// </summary>
        public override string Use()
        {
            return $"[Hammer] Молотком '{Name}' (вага: {Weight} кг) забито цвях у дерев'яну брусчатку.";
        }
    }

    /// <summary>
    /// Похідний клас: Викрутка.
    /// </summary>
    public class Screwdriver : Tool
    {
        public string TipType { get; set; } // Тип накінечника (наприклад, Phillips, Flathead)

        public Screwdriver(string name, string tipType) : base(name)
        {
            TipType = string.IsNullOrWhiteSpace(tipType) ? "Phillips (хрестоподібна)" : tipType;
        }

        /// <summary>
        /// Перевизначення (override) віртуального методу Use.
        /// </summary>
        public override string Use()
        {
            return $"[Screwdriver] Викруткою '{Name}' (тип шліца: '{TipType}') закручено саморіз у корпус.";
        }
    }

    /// <summary>
    /// Похідний клас: Гайковий ключ.
    /// </summary>
    public class Wrench : Tool
    {
        public int Size { get; set; } // Розмір ключа в мм

        public Wrench(string name, int size) : base(name)
        {
            Size = size > 0 ? size : 10;
        }

        /// <summary>
        /// Перевизначення (override) віртуального методу Use.
        /// </summary>
        public override string Use()
        {
            return $"[Wrench] Гайковим ключем '{Name}' (розмір: {Size} мм) затягнуто болтове з'єднання.";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("====================================================================");
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №8 (ВАРІАНТ 9)");
            Console.WriteLine("  Тема: Поліморфізм: динамічне зв’язування та перевизначення методів");
            Console.WriteLine("====================================================================\n");

            // 1. Створення поліморфної колекції List<Tool>
            List<Tool> toolbox = new List<Tool>
            {
                new Hammer("Stanley FatMax", 0.8),
                new Screwdriver("Wiha SoftFinish", "PH2 (Phillips)"),
                new Wrench("Craftsman Combo", 17),
                new Hammer("Важкий кувалдовий молот", 2.5),
                new Screwdriver("Pro'sKit Flat", "Flathead (плоска) 5mm"),
                new Wrench("Bahco Adjustable", 24)
            };

            Console.WriteLine($"У майстерні зібрано інструментів: {toolbox.Count} шт.\n");

            // 2. Використання поліморфізму та агрегація результатів
            Console.WriteLine("--- 1. Демонстрація поліморфних викликів Use() ---");
            
            // Список для агрегації (збору підсумкового списку дій)
            List<string> usageLog = new List<string>();

            int count = 1;
            foreach (Tool tool in toolbox)
            {
                // Завдяки динамічному зв'язуванню викликається override-метод
                // відповідного конкретного типу (Hammer, Screwdriver або Wrench)
                string actionResult = tool.Use();
                
                // Вивід у консоль
                Console.WriteLine($"{count}. {actionResult}");

                // Агрегація: зберігаємо результат поліморфного виклику
                usageLog.Add(actionResult);
                count++;
            }

            // 3. Підсумкова агрегація та аналітика
            Console.WriteLine("\n--- 2. Агрегація результатів поліморфних викликів ---");
            Console.WriteLine($"• Всього виконано операцій інструментами: {usageLog.Count}");
            
            // Підрахунок кількості інструментів кожного конкретного типу за допомогою Pattern Matching / IS
            int hammerCount = 0;
            int screwdriverCount = 0;
            int wrenchCount = 0;

            foreach (Tool tool in toolbox)
            {
                if (tool is Hammer) hammerCount++;
                else if (tool is Screwdriver) screwdriverCount++;
                else if (tool is Wrench) wrenchCount++;
            }

            Console.WriteLine($"• Використано молотків: {hammerCount}");
            Console.WriteLine($"• Використано викруток: {screwdriverCount}");
            Console.WriteLine($"• Використано гайкових ключів: {wrenchCount}");

            Console.WriteLine("\n====================================================================");
            Console.WriteLine("  ПОЯСНЕННЯ ДИНАМІЧНОГО ЗВ'ЯЗУВАННЯ:");
            Console.WriteLine("  Змінна циклу 'tool' має тип 'Tool' (базовий), але під час виконання");
            Console.WriteLine("  програми (Runtime) CLR звертається до таблиці віртуальних методів (VMT)");
            Console.WriteLine("  і викликає саме той 'Use()', який належить конкретному об'єкту в пам'яті.");
            Console.WriteLine("====================================================================");
        }
    }
}