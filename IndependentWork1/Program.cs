using System;
using System.Text;

namespace IndependentWork1
{
    /// <summary>
    /// Клас, що представляє кулінарний рецепт.
    /// </summary>
    public class Recipe
    {
        // 1. Приватні поля
        private string _title;
        private int _prepTimeMinutes;
        private int _servings;

        // 2. Властивості
        // Read-only властивість для назви рецепту
        public string Title => _title;

        public int PrepTimeMinutes
        {
            get => _prepTimeMinutes;
            private set => _prepTimeMinutes = value > 0 ? value : 10;
        }

        public int Servings
        {
            get => _servings;
            set => _servings = value > 0 ? value : 1;
        }

        // 3. Конструктор
        public Recipe(string title, int prepTimeMinutes, int servings)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Назва рецепту не може бути порожньою.", nameof(title));

            _title = title;
            PrepTimeMinutes = prepTimeMinutes;
            Servings = servings;
        }

        // 4. Метод з обчисленням
        /// <summary>
        /// Обчислює загальний час приготування для вказаної кількості партій.
        /// </summary>
        public int GetTotalTimeForBatches(int batchCount)
        {
            if (batchCount <= 0) return PrepTimeMinutes;
            return PrepTimeMinutes * batchCount;
        }
    }

    /// <summary>
    /// Клас, що представляє музичний плейлист.
    /// </summary>
    public class Playlist
    {
        // 1. Приватні поля
        private string _name;
        private int _trackCount;
        private int _totalDurationSeconds;

        // 2. Властивості
        public string Name => _name;

        public int TrackCount
        {
            get => _trackCount;
            set => _trackCount = value >= 0 ? value : 0;
        }

        public int TotalDurationSeconds
        {
            get => _totalDurationSeconds;
            private set => _totalDurationSeconds = value >= 0 ? value : 0;
        }

        // 3. Конструктор
        public Playlist(string name, int trackCount, int totalDurationSeconds)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва плейлиста не може бути порожньою.", nameof(name));

            _name = name;
            TrackCount = trackCount;
            TotalDurationSeconds = totalDurationSeconds;
        }

        // 4. Метод з форматуванням часу
        /// <summary>
        /// Повертає тривалість плейлиста у форматі "X год Y хв".
        /// </summary>
        public string GetFormattedDuration()
        {
            int hours = TotalDurationSeconds / 3600;
            int minutes = (TotalDurationSeconds % 3600) / 60;
            return $"{hours} год {minutes} хв";
        }
    }

    /// <summary>
    /// Клас, що представляє працівника компанії.
    /// </summary>
    public class Employee
    {
        // 1. Приватні поля
        private string _fullName;
        private double _baseSalary;
        private bool _isRemote;

        // 2. Властивості
        public string FullName => _fullName;

        public double BaseSalary
        {
            get => _baseSalary;
            set => _baseSalary = value >= 0 ? value : 0;
        }

        public bool IsRemote
        {
            get => _isRemote;
            set => _isRemote = value;
        }

        // 3. Конструктор
        public Employee(string fullName, double baseSalary, bool isRemote)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("ПІБ працівника не може бути порожнім.", nameof(fullName));

            _fullName = fullName;
            BaseSalary = baseSalary;
            IsRemote = isRemote;
        }

        // 4. Метод з розрахунком бонусу
        /// <summary>
        /// Обчислює премію працівника за коефіцієнтом ефективності (KPI).
        /// </summary>
        public double CalculateBonus(double performanceFactor)
        {
            if (performanceFactor < 0) performanceFactor = 0;
            
            // Якщо працює дистанційно, додається невеликий бонус за екологічність
            double remoteAllowance = IsRemote ? 500.0 : 0.0;
            return (BaseSalary * performanceFactor) + remoteAllowance;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("====================================================================");
            Console.WriteLine("  САМОСТІЙНА РОБОТА №1");
            Console.WriteLine("  Тема: Базовий синтаксис C# і оголошення класів");
            Console.WriteLine("====================================================================\n");

            // --- 1. Демонстрація класу Recipe ---
            Console.WriteLine("--- 1. Демонстрація класу Recipe (Рецепт) ---");
            Recipe borsch = new Recipe("Український Борщ", 90, 6);
            int doubleBatchTime = borsch.GetTotalTimeForBatches(2);

            Console.WriteLine($"• Рецепт: '{borsch.Title}'");
            Console.WriteLine($"• Час приготування однієї порційної партії: {borsch.PrepTimeMinutes} хв");
            Console.WriteLine($"• Розрахована кількість порцій: {borsch.Servings}");
            Console.WriteLine($"• Сумарний час для приготування 2-х великих партій: {doubleBatchTime} хв ({doubleBatchTime / 60.0:F1} год)\n");

            // --- 2. Демонстрація класу Playlist ---
            Console.WriteLine("--- 2. Демонстрація класу Playlist (Музичний плейлист) ---");
            Playlist rockPlaylist = new Playlist("Classic Rock Hits", 24, 5240); // 5240 сек = 1 год 27 хв
            string formattedTime = rockPlaylist.GetFormattedDuration();

            Console.WriteLine($"• Назва плейлиста: '{rockPlaylist.Name}'");
            Console.WriteLine($"• Кількість треків: {rockPlaylist.TrackCount}");
            Console.WriteLine($"• Загальний час звучання у секундах: {rockPlaylist.TotalDurationSeconds} сек");
            Console.WriteLine($"• Зручний формат тривалості: {formattedTime}\n");

            // --- 3. Демонстрація класу Employee ---
            Console.WriteLine("--- 3. Демонстрація класу Employee (Працівник) ---");
            Employee dev = new Employee("Олександр Шевченко", 35000.0, true);
            double kpiFactor = 0.20; // 20% премії за виконання плану
            double bonusAmount = dev.CalculateBonus(kpiFactor);
            double totalPayment = dev.BaseSalary + bonusAmount;

            Console.WriteLine($"• Працівник: {dev.FullName}");
            Console.WriteLine($"• Формат роботи: {(dev.IsRemote ? "Дистанційно (Remote)" : "В офісі")}");
            Console.WriteLine($"• Базовий оклад: {dev.BaseSalary:N2} грн");
            Console.WriteLine($"• Нарахована премія (KPI {kpiFactor * 100}% + надбавка): {bonusAmount:N2} грн");
            Console.WriteLine($"• Загальна виплата до нарахування: {totalPayment:N2} грн");

            Console.WriteLine("\n====================================================================");
            Console.WriteLine("  Програму успішно виконано.");
            Console.WriteLine("====================================================================");
        }
    }
}