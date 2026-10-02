using System;
using System.Text;

namespace IndependentWork2
{
    /// <summary>
    /// Клас, що представляє товар в інтернет-магазині.
    /// </summary>
    public class Product
    {
        // 1. Приватні поля
        private readonly int _id;
        private readonly string _name;
        private readonly decimal _price;
        private readonly string _category;
        private readonly int _stockCount;

        // 2. Публічні read-only властивості
        public int Id => _id;
        public string Name => _name;
        public decimal Price => _price;
        public string Category => _category;
        public int StockCount => _stockCount;

        // 3. Перевантажені конструктори

        // Конструктор 1 (основний)
        public Product(int id, string name, decimal price, string category, int stockCount)
        {
            _id = id;
            _name = string.IsNullOrWhiteSpace(name) ? "Без назви" : name;
            _price = price >= 0 ? price : 0;
            _category = string.IsNullOrWhiteSpace(category) ? "Uncategorized" : category;
            _stockCount = stockCount >= 0 ? stockCount : 0;
        }

        // Конструктор 2 (для швидкого додавання товару)
        // Викликає основний конструктор, встановлюючи значення за замовчуванням
        public Product(int id, string name, decimal price)
            : this(id, name, price, "Uncategorized", 0)
        {
        }

        // Конструктор 3 (конструктор копіювання)
        // Приймає інший екземпляр Product та викликає основний конструктор
        public Product(Product other)
            : this(
                other ?? throw new ArgumentNullException(nameof(other), "Об'єкт для копіювання не може бути null"),
                other.Id,
                other.Name,
                other.Price,
                other.Category,
                other.StockCount)
        {
        }

        // Допоміжний приватний конструктор для забезпечення безпечного виклику this(...) під час розпакування other
        private Product(Product other, int id, string name, decimal price, string category, int stockCount)
            : this(id, name, price, category, stockCount)
        {
        }

        // 4. Перевизначення методу ToString()
        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Price: {Price:C}, Category: {Category}, Stock: {StockCount}";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Налаштування виводу юнікод-символів (для некоректного відображення гривні/валюти у терміналі)
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("==========================================================================================");
            Console.WriteLine("  САМОСТІЙНА РОБОТА №2");
            Console.WriteLine("  Тема: Перевантаження конструкторів: приклади та сценарії");
            Console.WriteLine("==========================================================================================\n");

            Console.WriteLine("Створення товарів:\n");

            // 1. Створення товару за допомогою основного конструктора
            Product laptop = new Product(101, "Laptop", 35000.00m, "Electronics", 15);
            Console.WriteLine($"Товар 1 (основний конструктор): {laptop}");

            // 2. Створення товару за допомогою скороченого конструктора
            Product mouse = new Product(102, "Mouse", 800.00m);
            Console.WriteLine($"Товар 2 (скорочений конструктор): {mouse}");

            // 3. Створення товару за допомогою конструктора копіювання
            Product laptopCopy = new Product(laptop);
            Console.WriteLine($"Товар 3 (конструктор копіювання): {laptopCopy}");

            Console.WriteLine("\n==========================================================================================");
            Console.WriteLine("  Програму успішно виконано.");
            Console.WriteLine("==========================================================================================");
        }
    }
}