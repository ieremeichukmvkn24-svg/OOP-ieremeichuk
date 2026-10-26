using System;
using System.Collections.Generic;

namespace lab11v9
{
    // Абстрактний базовий клас (Template Method Pattern)
    public abstract class ImageProcessor
    {
        public string ImageName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsLoaded { get; protected set; } = false;

        protected ImageProcessor(string imageName, int width, int height)
        {
            ImageName = imageName;
            Width = width;
            Height = height;
        }

        // Шаблонний метод (визначає незмінний скелет алгоритму)
        public void Process()
        {
            Console.WriteLine($"=== Початок обробки зображення: {ImageName} ===");
            Load();
            Resize();
            ApplyFilter();
            AdjustColors(); // Віртуальний метод з базовою реалізацією
            Save();
            Console.WriteLine($"=== Обробку зображення {ImageName} завершено ===\n");
        }

        // Обов'язкові абстрактні кроки для реалізації в похідних класах
        protected abstract void Load();
        protected abstract void Resize();
        protected abstract void ApplyFilter();
        protected abstract void Save();

        // Опціональний віртуальний крок з базовою реалізацією
        protected virtual void AdjustColors()
        {
            Console.WriteLine($"[Базовий клас] Коригування кольорів за замовчуванням для '{ImageName}'.");
        }
    }

    // Похідний клас 1: Обробник для створення мініатюр (Thumbnail)
    public class ThumbnailProcessor : ImageProcessor
    {
        public int TargetWidth { get; set; } = 150;
        public int TargetHeight { get; set; } = 150;

        public ThumbnailProcessor(string imageName, int width, int height) 
            : base(imageName, width, height) { }

        protected override void Load()
        {
            Console.WriteLine($"1. Завантаження зображення '{ImageName}' ({Width}x{Height}) для мініатюри.");
            IsLoaded = true;
        }

        protected override void Resize()
        {
            Console.WriteLine($"2. Зміна розміру зображення до мініатюри: {TargetWidth}x{TargetHeight}.");
            Width = TargetWidth;
            Height = TargetHeight;
        }

        protected override void ApplyFilter()
        {
            Console.WriteLine("3. Застосування шарпнінг-фільтра для покращення чіткості мініатюри.");
        }

        protected override void Save()
        {
            Console.WriteLine($"5. Збереження мініатюри як 'thumb_{ImageName}'.");
        }
    }

    // Похідний клас 2: Обробник для накладання водяного знаку (Watermark)
    public class WatermarkProcessor : ImageProcessor
    {
        public string WatermarkText { get; set; } = "Confidential";

        public WatermarkProcessor(string imageName, int width, int height, string watermarkText) 
            : base(imageName, width, height)
        {
            WatermarkText = watermarkText;
        }

        protected override void Load()
        {
            Console.WriteLine($"1. Завантаження зображення '{ImageName}' ({Width}x{Height}) для водяного знаку.");
            IsLoaded = true;
        }

        protected override void Resize()
        {
            Console.WriteLine("2. Зміна розміру не потрібна (пропускається для оригінального документу).");
        }

        protected override void ApplyFilter()
        {
            Console.WriteLine($"3. Накладання водяного знаку з текстом: '{WatermarkText}'.");
        }

        protected override void Save()
        {
            Console.WriteLine($"5. Збереження зображення з водяним знаком у форматі PNG.");
        }
    }

    // Похідний клас 3: Обробник для художніх фільтрів (FilterProcessor)
    public class FilterProcessor : ImageProcessor
    {
        public string FilterName { get; set; } = "Sepia";

        public FilterProcessor(string imageName, int width, int height, string filterName) 
            : base(imageName, width, height)
        {
            FilterName = filterName;
        }

        protected override void Load()
        {
            Console.WriteLine($"1. Завантаження зображення '{ImageName}' ({Width}x{Height}) для фільтрації.");
            IsLoaded = true;
        }

        protected override void Resize()
        {
            Console.WriteLine("2. Масштабування зображення під стандартний роздільний дозвіл екрана.");
        }

        protected override void ApplyFilter()
        {
            Console.WriteLine($"3. Застосування художнього фільтра '{FilterName}'.");
        }

        // Перевизначення віртуального методу для унікальної поведінки
        protected override void AdjustColors()
        {
            Console.WriteLine($"[Специфічно для FilterProcessor] Тонке налаштування балансу білого для фільтра '{FilterName}'.");
        }

        protected override void Save()
        {
            Console.WriteLine($"5. Збереження відфільтрованого зображення як '{FilterName}_{ImageName}'.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Створення списку об'єктів базового типу ImageProcessor
            List<ImageProcessor> processors = new List<ImageProcessor>
            {
                new ThumbnailProcessor("photo1.jpg", 1920, 1080),
                new WatermarkProcessor("document_scan.png", 1200, 1600, "Property of IT College"),
                new FilterProcessor("sunset.jpg", 3840, 2160, "VintageSepia")
            };

            // Демонстрація поліморфного виклику шаблонного методу Process()
            foreach (var processor in processors)
            {
                processor.Process();
            }
        }
    }
}