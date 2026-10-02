using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lab5Variant9
{
    /// <summary>
    /// Представляє розріджений вектор, що зберігає лише ненульові значення у словнику (індекс -> значення).
    /// </summary>
    public class SparseVector
    {
        // Приватне поле для зберігання даних
        private readonly Dictionary<int, double> _data;

        /// <summary>
        /// Кількість ненульових елементів у векторі.
        /// </summary>
        public int Count => _data.Count;

        /// <summary>
        /// Конструктор за замовчуванням або з початковими елементами.
        /// </summary>
        public SparseVector()
        {
            _data = new Dictionary<int, double>();
        }

        /// <summary>
        /// Конструктор для ініціалізації із словника.
        /// </summary>
        public SparseVector(IDictionary<int, double> initialData) : this()
        {
            if (initialData is null)
                throw new ArgumentNullException(nameof(initialData));

            foreach (var kvp in initialData)
            {
                this[kvp.Key] = kvp.Value; // Використовуємо індексатор для збереження з валідацією
            }
        }

        // Індексатор для доступу за індексом
        public double this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Індекс не може бути від'ємним.");

                return _data.TryGetValue(index, out double value) ? value : 0.0;
            }
            set
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Індекс не може бути від'ємним.");

                // Якщо значення 0, видаляємо з розрідженого масиву для економії пам'яті
                if (Math.Abs(value) < 1e-9)
                {
                    _data.Remove(index);
                }
                else
                {
                    _data[index] = value;
                }
            }
        }

        // Перевантаження оператора + (додавання двох векторів)
        public static SparseVector operator +(SparseVector v1, SparseVector v2)
        {
            if (v1 is null) throw new ArgumentNullException(nameof(v1));
            if (v2 is null) throw new ArgumentNullException(nameof(v2));

            var result = new SparseVector();

            // Додаємо всі елементи першого вектора
            foreach (var kvp in v1._data)
            {
                result[kvp.Key] = kvp.Value;
            }

            // Додаємо елементи другого вектора
            foreach (var kvp in v2._data)
            {
                result[kvp.Key] += kvp.Value;
            }

            return result;
        }

        // Перевантаження оператора * (скалярний добуток двох векторів)
        public static double operator *(SparseVector v1, SparseVector v2)
        {
            if (v1 is null) throw new ArgumentNullException(nameof(v1));
            if (v2 is null) throw new ArgumentNullException(nameof(v2));

            double dotProduct = 0.0;

            // Ітеруємося по меншому за розміром словнику для оптимізації
            var (smaller, larger) = v1.Count < v2.Count ? (v1, v2) : (v2, v1);

            foreach (var kvp in smaller._data)
            {
                if (larger._data.TryGetValue(kvp.Key, out double otherVal))
                {
                    dotProduct += kvp.Value * otherVal;
                }
            }

            return dotProduct;
        }

        // Перевантаження операторів порівняння == та !=
        public static bool operator ==(SparseVector? left, SparseVector? right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left is null || right is null) return false;

            if (left.Count != right.Count) return false;

            foreach (var kvp in left._data)
            {
                if (!right._data.TryGetValue(kvp.Key, out double rightVal) || Math.Abs(kvp.Value - rightVal) > 1e-9)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool operator !=(SparseVector? left, SparseVector? right) => !(left == right);

        // Перевизначення Equals, GetHashCode та ToString
        public override bool Equals(object? obj)
        {
            if (obj is SparseVector other)
            {
                return this == other;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var kvp in _data.OrderBy(k => k.Key))
            {
                hash = hash * 31 + kvp.Key.GetHashCode();
                hash = hash * 31 + kvp.Value.GetHashCode();
            }
            return hash;
        }

        public override string ToString()
        {
            if (_data.Count == 0)
                return "SparseVector [Порожній / всі елементи 0]";

            var sb = new StringBuilder("SparseVector {\n");
            foreach (var kvp in _data.OrderBy(k => k.Key))
            {
                sb.AppendLine($"  [{kvp.Key}] => {kvp.Value}");
            }
            sb.Append('}');
            return sb.ToString();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ КЛАСУ SPARSEVECTOR (ВАРАНТ 9) ===\n");

            // 1. Створення об'єктів та робота з індексатором (Запис)
            Console.WriteLine("--- 1. Створення векторів та запис через індексатор ---");
            SparseVector v1 = new SparseVector();
            v1[2] = 5.5;
            v1[100] = 3.2;
            v1[1000] = 10.0;

            SparseVector v2 = new SparseVector();
            v2[2] = 4.5;
            v2[50] = 7.0;
            v2[1000] = -2.0;

            Console.WriteLine($"Вектор v1 (ненульових елементів: {v1.Count}):\n{v1}");
            Console.WriteLine($"\nВектор v2 (ненульових елементів: {v2.Count}):\n{v2}\n");

            // 2. Робота з індексатором (Читання та скидання в 0)
            Console.WriteLine("--- 2. Читання через індексатор та автоматичне видалення при присвоєнні 0 ---");
            Console.WriteLine($"v1[2] = {v1[2]} (існує)");
            Console.WriteLine($"v1[5] = {v1[5]} (за замовчуванням 0 для неіснуючого індексу)");

            Console.WriteLine("\nПрисвоюємо v1[100] = 0.0 (елемент має видалитися зі словника)...");
            v1[100] = 0.0;
            Console.WriteLine($"Оновлений v1 (ненульових елементів: {v1.Count}):\n{v1}\n");

            // 3. Оператор додавання (+)
            Console.WriteLine("--- 3. Перевантажений оператор додавання (+) ---");
            SparseVector vSum = v1 + v2;
            Console.WriteLine($"v1 + v2 (ненульових елементів: {vSum.Count}):\n{vSum}\n");

            // 4. Оператор скалярного добутку (*)
            Console.WriteLine("--- 4. Перевантажений оператор скалярного добутку (*) ---");
            // v1: [2]=>5.5, [1000]=>10.0
            // v2: [2]=>4.5, [50]=>7.0, [1000]=>-2.0
            // Очікуваний добуток: (5.5 * 4.5) + (10.0 * -2.0) = 24.75 - 20.0 = 4.75
            double dotProduct = v1 * v2;
            Console.WriteLine($"v1 * v2 = {dotProduct}\n");

            // 5. Порівняння операторами == та !=, та методами Equals/GetHashCode
            Console.WriteLine("--- 5. Порівняння векторів (==, !=, Equals, GetHashCode) ---");
            SparseVector v3 = new SparseVector();
            v3[2] = 5.5;
            v3[1000] = 10.0;

            Console.WriteLine($"v1 == v3: {v1 == v3}");
            Console.WriteLine($"v1 != v2: {v1 != v2}");
            Console.WriteLine($"v1.Equals(v3): {v1.Equals(v3)}");
            Console.WriteLine($"v1.GetHashCode() == v3.GetHashCode(): {v1.GetHashCode() == v3.GetHashCode()}");

            // 6. Перевірка обробки помилок індексатора
            Console.WriteLine("\n--- 6. Перевірка валідації від'ємного індексу ---");
            try
            {
                Console.WriteLine("Спроба звернутися до v1[-5]...");
                double val = v1[-5];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"[Перехоплено виключення]: {ex.Message}");
            }

            Console.WriteLine("\nЗавершення роботи програми.");
        }
    }
}