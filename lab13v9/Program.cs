using System;
using System.Collections.Generic;

namespace lab13v9
{
    // Тестовий клас відповідно до попередніх лабораторних (Варіант 9)
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }

        public Project(int id, string name, DateTime startDate)
        {
            Id = id;
            Name = name;
            StartDate = startDate;
        }

        public override string ToString()
        {
            return $"Project [Id={Id}, Name={Name}, StartDate={StartDate.ToShortDateString()}]";
        }
    }

    // Клас з узагальненими функціями вищого порядку
    public static class FunctionalUtils
    {
        // 1. Map: Перетворює колекцію List<T> у List<TResult> за допомогою Func<T, TResult>
        public static List<TResult> Map<T, TResult>(List<T> list, Func<T, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in list)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        // 2. ForEach: Виконує Action<T> для кожного елемента колекції
        public static void ForEach<T>(List<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        // 3. Any: Перевіряє, чи хоча б один елемент задовольняє умові (Predicate<T>)
        public static bool Any<T>(List<T> list, Predicate<T> predicate)
        {
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    return true;
                }
            }
            return false;
        }

        // 4. All: Перевіряє, чи всі елементи задовольняють умові (Predicate<T>)
        public static bool All<T>(List<T> list, Predicate<T> predicate)
        {
            foreach (var item in list)
            {
                if (!predicate(item))
                {
                    return false;
                }
            }
            return true;
        }

        // 5. Contains: Перевіряє, чи містить колекція заданий елемент (з урахуванням IEqualityComparer / Equals)
        public static bool Contains<T>(List<T> list, T value)
        {
            foreach (var item in list)
            {
                if (EqualityComparer<T>.Default.Equals(item, value))
                {
                    return true;
                }
            }
            return false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // 1. Демонстрація з колекцією чисел (int)
            List<int> numbers = new List<int> { 2, 4, 6, 8, 10 };
            Console.WriteLine($"Числа: {string.Join(", ", numbers)}");

            // Map: помножити кожен елемент на 2
            var doubled = FunctionalUtils.Map(numbers, x => x * 2);
            Console.WriteLine($"Map (x * 2): {string.Join(", ", doubled)}");

            // ForEach: вивести числа в консоль
            Console.Write("ForEach вивід: ");
            FunctionalUtils.ForEach(numbers, x => Console.Write($"{x} "));
            Console.WriteLine();

            // Any: чи є числа більші за 9?
            bool hasGreaterThanNine = FunctionalUtils.Any(numbers, x => x > 9);
            Console.WriteLine($"Any (x > 9): {hasGreaterThanNine}");

            // All: чи всі числа парні?
            bool allEven = FunctionalUtils.All(numbers, x => x % 2 == 0);
            Console.WriteLine($"All (парні): {allEven}");

            // Contains: чи містить число 6?
            bool containsSix = FunctionalUtils.Contains(numbers, 6);
            Console.WriteLine($"Contains (6): {containsSix}\n");

            // 2. Демонстрація з колекцією рядків (string)
            List<string> words = new List<string> { "Csharp", "Generics", "Func", "Action" };
            Console.WriteLine($"Слова: {string.Join(", ", words)}");

            // Map: отримати довжину кожного слова
            var lengths = FunctionalUtils.Map(words, w => w.Length);
            Console.WriteLine($"Map (довжини слів): {string.Join(", ", lengths)}");

            // All: чи всі слова мають довжину більше 2?
            bool allLongerThanTwo = FunctionalUtils.All(words, w => w.Length > 2);
            Console.WriteLine($"All (довжина > 2): {allLongerThanTwo}");

            // Contains: чи містить рядок "Func"?
            bool containsFunc = FunctionalUtils.Contains(words, "Func");
            Console.WriteLine($"Contains (\"Func\"): {containsFunc}\n");

            // 3. Демонстрація з власним класом (Project)
            List<Project> projects = new List<Project>
            {
                new Project(1, "Alpha System", new DateTime(2026, 1, 10)),
                new Project(2, "Beta Platform", new DateTime(2026, 3, 15)),
                new Project(3, "Gamma Cloud", new DateTime(2026, 5, 20))
            };

            Console.WriteLine("Список проєктів:");
            FunctionalUtils.ForEach(projects, p => Console.WriteLine($" - {p}"));

            // Map: отримати тільки назви проєктів
            var projectNames = FunctionalUtils.Map(projects, p => p.Name);
            Console.WriteLine($"\nMap (назви проєктів): {string.Join(", ", projectNames)}");

            // Any: чи є проєкт з назвою "Beta Platform"?
            bool hasBeta = FunctionalUtils.Any(projects, p => p.Name == "Beta Platform");
            Console.WriteLine($"Any (назва 'Beta Platform'): {hasBeta}");
        }
    }
}