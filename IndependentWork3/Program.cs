using System;
using System.Text;

namespace IndependentWork1
{
    public class Recipe
    {
        private string _title;
        private int _prepTimeMinutes;
        private int _servings;

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

        public Recipe(string title, int prepTimeMinutes, int servings)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Назва рецепту не може бути порожньою.", nameof(title));

            _title = title;
            PrepTimeMinutes = prepTimeMinutes;
            Servings = servings;
        }

        public int GetTotalTimeForBatches(int batchCount)
        {
            if (batchCount <= 0) return PrepTimeMinutes;
            return PrepTimeMinutes * batchCount;
        }
    }

    public class Playlist
    {
        private string _name;
        private int _trackCount;
        private int _totalDurationSeconds;

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

        public Playlist(string name, int trackCount, int totalDurationSeconds)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва плейлиста не може бути порожньою.", nameof(name));

            _name = name;
            TrackCount = trackCount;
            TotalDurationSeconds = totalDurationSeconds;
        }

        public string GetFormattedDuration()
        {
            int hours = TotalDurationSeconds / 3600;
            int minutes = (TotalDurationSeconds % 3600) / 60;
            return $"{hours} год {minutes} хв";
        }
    }

    public class Employee
    {
        private string _fullName;
        private double _baseSalary;
        private bool _isRemote;

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

        public Employee(string fullName, double baseSalary, bool isRemote)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("ПІБ працівника не може бути порожнім.", nameof(fullName));

            _fullName = fullName;
            BaseSalary = baseSalary;
            IsRemote = isRemote;
        }

        public double CalculateBonus(double performanceFactor)
        {
            if (performanceFactor < 0) performanceFactor = 0;
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
            Console.WriteLine("  САМОСТІЙНА РОБОТА №3");
            Console.WriteLine("====================================================================\n");

            // 1. Recipe
            Recipe borsch = new Recipe("Український Борщ", 90, 6);
            int doubleBatchTime = borsch.GetTotalTimeForBatches(2);
            Console.WriteLine($"• Рецепт: '{borsch.Title}', час для 2 партій: {doubleBatchTime} хв");

            // 2. Playlist
            Playlist rockPlaylist = new Playlist("Classic Rock Hits", 24, 5240);
            Console.WriteLine($"• Плейлист: '{rockPlaylist.Name}', тривалість: {rockPlaylist.GetFormattedDuration()}");

            // 3. Employee
            Employee dev = new Employee("Олександр Шевченко", 35000.0, true);
            double bonusAmount = dev.CalculateBonus(0.20);
            Console.WriteLine($"• Працівник: {dev.FullName}, оклад + премія: {dev.BaseSalary + bonusAmount:N2} грн");
        }
    }
}