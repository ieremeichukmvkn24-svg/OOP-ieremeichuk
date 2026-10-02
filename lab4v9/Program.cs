using System;

namespace Lab4Variant9
{
    /// <summary>
    /// Представляє лічильник з валідацією значення, статичною межею та перевантаженими операторами.
    /// </summary>
    public class Counter
    {
        // Приватні поля
        private int _value;
        private int _step;

        // Статичні члени класу
        public static int MaxCount { get; set; } = 100;

        // Властивості з валідацією
        public int Value
        {
            get => _value;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Значення лічильника не може бути від'ємним!");
                }
                if (value > MaxCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"Значення лічильника не може перевищувати MaxCount ({MaxCount})!");
                }
                _value = value;
            }
        }

        public int Step
        {
            get => _step;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Крок лічильника має бути більшим за 0!", nameof(value));
                }
                _step = value;
            }
        }

        // Конструктор
        public Counter(int initialValue = 0, int step = 1)
        {
            Step = step;
            Value = initialValue; // Використовуємо сеттер властивості для проходження валідації
        }

        // Індексатор
        // 0 - повертає/змінює Value
        // 1 - повертає/змінює Step
        public int this[int index]
        {
            get => index switch
            {
                0 => Value,
                1 => Step,
                _ => throw new IndexOutOfRangeException("Допустимі індекси: 0 (Value), 1 (Step).")
            };
            set
            {
                switch (index)
                {
                    case 0:
                        Value = value;
                        break;
                    case 1:
                        Step = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Допустимі індекси: 0 (Value), 1 (Step).");
                }
            }
        }

        // Перевантаження унарного оператора ++ (інкремент)
        public static Counter operator ++(Counter c)
        {
            if (c is null)
                throw new ArgumentNullException(nameof(c));

            int newValue = c.Value + c.Step;
            if (newValue > MaxCount)
            {
                Console.WriteLine($"[Увага] Досягнуто MaxCount ({MaxCount})! Значення встановлено на MaxCount.");
                newValue = MaxCount;
            }

            return new Counter(newValue, c.Step);
        }

        // Перевантаження унарного оператора -- (декремент)
        public static Counter operator --(Counter c)
        {
            if (c is null)
                throw new ArgumentNullException(nameof(c));

            int newValue = c.Value - c.Step;
            if (newValue < 0)
            {
                Console.WriteLine("[Увага] Значення не може бути від'ємним! Встановлено 0.");
                newValue = 0;
            }

            return new Counter(newValue, c.Step);
        }

        // Перевантаження операторів порівняння == та !=
        public static bool operator ==(Counter? left, Counter? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;

            return left.Value == right.Value && left.Step == right.Step;
        }

        public static bool operator !=(Counter? left, Counter? right) => !(left == right);

        // Перевизначення Equals, GetHashCode та ToString
        public override bool Equals(object? obj)
        {
            if (obj is Counter other)
            {
                return this == other;
            }
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Value, Step);

        public override string ToString() => $"Counter [Value: {Value}, Step: {Step}, MaxCount: {MaxCount}]";
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ КЛАСУ COUNTER (ВАРАНТ 9) ===\n");

            // 1. Демонстрація створення об'єктів та статичного члена
            Console.WriteLine($"--- 1. Статичний член MaxCount = {Counter.MaxCount} ---");
            Counter c1 = new Counter(10, 5);
            Counter c2 = new Counter(10, 5);
            Counter c3 = new Counter(95, 10);

            Console.WriteLine($"c1: {c1}");
            Console.WriteLine($"c2: {c2}");
            Console.WriteLine($"c3: {c3}\n");

            // 2. Демонстрація валідації
            Console.WriteLine("--- 2. Перевірка валідації властивостей ---");
            try
            {
                Console.WriteLine("Спроба присвоїти від'ємне значення (-5)...");
                c1.Value = -5;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"[Перехоплено виключення]: {ex.Message}");
            }

            try
            {
                Console.WriteLine("Спроба перевищити MaxCount (150)...");
                c1.Value = 150;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"[Перехоплено виключення]: {ex.Message}\n");
            }

            // 3. Демонстрація індексатора
            Console.WriteLine("--- 3. Робота з індексатором ---");
            Console.WriteLine($"Початковий c1[0] (Value): {c1[0]}, c1[1] (Step): {c1[1]}");
            c1[0] = 20; // змінюємо Value
            c1[1] = 2;  // змінюємо Step
            Console.WriteLine($"Оновлений c1 через індексатор: {c1}\n");

            // 4. Демонстрація перевантажених операторів ++ та --
            Console.WriteLine("--- 4. Перевантажені оператори ++ та -- ---");
            Console.WriteLine($"c1 до ++: {c1}");
            c1++;
            Console.WriteLine($"c1 після ++: {c1}");
            c1--;
            Console.WriteLine($"c1 після --: {c1}");

            Console.WriteLine($"\nСпроба ++ для c3 ({c3}), що перевищить MaxCount:");
            c3++;
            Console.WriteLine($"c3 після ++: {c3}\n");

            // 5. Демонстрація операторів порівняння == та != та методів Equals/GetHashCode
            Console.WriteLine("--- 5. Порівняння об'єктів (==, !=, Equals) ---");
            Console.WriteLine($"c1: {c1}");
            Console.WriteLine($"c2: {c2}");
            Console.WriteLine($"c1 == c2: {c1 == c2}");
            Console.WriteLine($"c1 != c2: {c1 != c2}");
            Console.WriteLine($"c1.Equals(c2): {c1.Equals(c2)}");
            Console.WriteLine($"c1.GetHashCode() == c2.GetHashCode(): {c1.GetHashCode() == c2.GetHashCode()}");

            Console.WriteLine("\nЗавершення роботи програми.");
        }
    }
}