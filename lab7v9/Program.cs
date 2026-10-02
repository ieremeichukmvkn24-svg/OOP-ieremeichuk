using System;
using System.Collections.Generic;

namespace Lab7Variant9
{
    /// <summary>
    /// Базовий клас, що представляє загальний медіафайл.
    /// </summary>
    public class Media
    {
        private string _title;
        private int _durationSeconds;

        public string Title
        {
            get => _title;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва медіафайлу не може бути порожньою.", nameof(value));
                _title = value;
            }
        }

        public int DurationSeconds
        {
            get => _durationSeconds;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Тривалість не може бути від'ємною.", nameof(value));
                _durationSeconds = value;
            }
        }

        public Media(string title, int durationSeconds)
        {
            Title = title;
            DurationSeconds = durationSeconds;
        }

        /// <summary>
        /// Віртуальний метод для відтворення медіафайлу.
        /// </summary>
        public virtual void Play()
        {
            Console.WriteLine($"[Media] Відтворення базового медіафайлу: '{Title}' ({DurationSeconds} сек)");
        }
    }

    /// <summary>
    /// Похідний клас A: Відеофайл (використовує OVERRIDE для перевизначення Play).
    /// </summary>
    public class Video : Media
    {
        public string Resolution { get; set; }

        public Video(string title, int durationSeconds, string resolution)
            : base(title, durationSeconds)
        {
            Resolution = string.IsNullOrWhiteSpace(resolution) ? "1080p" : resolution;
        }

        /// <summary>
        /// Перевизначення (override) віртуального методу Play.
        /// Забезпечує динамічне зв'язування (поліморфізм).
        /// </summary>
        public override void Play()
        {
            Console.WriteLine($"[Video (override)] Відтворення відео: '{Title}' у роздільності {Resolution} [{DurationSeconds} сек]");
        }
    }

    /// <summary>
    /// Похідний клас B: Аудіофайл (використовує NEW для приховування Play).
    /// </summary>
    public class Audio : Media
    {
        public int BitrateKbps { get; set; }

        public Audio(string title, int durationSeconds, int bitrateKbps)
            : base(title, durationSeconds)
        {
            BitrateKbps = bitrateKbps > 0 ? bitrateKbps : 320;
        }

        /// <summary>
        /// Приховування (new) методу базового класу Play.
        /// Забезпечує статичне зв'язування на основі типу посилання.
        /// </summary>
        public new void Play()
        {
            Console.WriteLine($"[Audio (new)] Програвання аудіо: '{Title}' із бітрейтом {BitrateKbps} kbps [{DurationSeconds} сек]");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("====================================================================");
            Console.WriteLine("  ЛАБОРАТОРНА РОБОТА №7 (ВАРАНТ 9)");
            Console.WriteLine("  Тема: Приховування методів (new) vs перевизначення (override)");
            Console.WriteLine("====================================================================\n");

            // 1. Створення об'єктів безпосередніх типів
            Video directVideo = new Video("Inception Trailer", 150, "4K Ultra HD");
            Audio directAudio = new Audio("Bohemian Rhapsody", 354, 320);

            // 2. Збереження об'єктів у змінних типу базового класу (Upcasting)
            Media mediaRefToVideo = directVideo; // Video -> Media
            Media mediaRefToAudio = directAudio; // Audio -> Media

            // --- ДЕМОНСТРАЦІЯ 1: Виклики через посилання БАЗОВОГО типу (Media) ---
            Console.WriteLine("--- 1. Виклик методу Play() через посилання БАЗОВОГО типу (Media) ---");
            Console.Write("  • mediaRefToVideo.Play(): ");
            mediaRefToVideo.Play(); // Викликає Video.Play() за рахунок OVERRIDE (Поліморфізм!)

            Console.Write("  • mediaRefToAudio.Play(): ");
            mediaRefToAudio.Play(); // Викликає Media.Play() через NEW (Приховування розірвало поліморфний ланцюг!)
            Console.WriteLine();

            // --- ДЕМОНСТРАЦІЯ 2: Виклики через посилання ПОХІДНИХ типів (Downcasting) ---
            Console.WriteLine("--- 2. Виклик методу Play() через посилання ПОХІДНИХ типів (Downcasting) ---");
            Console.Write("  • ((Video)mediaRefToVideo).Play(): ");
            ((Video)mediaRefToVideo).Play(); // Викликає Video.Play()

            Console.Write("  • ((Audio)mediaRefToAudio).Play(): ");
            ((Audio)mediaRefToAudio).Play(); // Викликає Audio.Play() (Явне приведення відкриває прихований метод)
            Console.WriteLine();

            // --- ДЕМОНСТРАЦІЯ 3: Робота в колекції List<Media> ---
            Console.WriteLine("--- 3. Демонстрація роботи списку List<Media> (Плейлист) ---");
            List<Media> playlist = new List<Media>
            {
                new Media("Універсальна аудіодоріжка", 60),
                new Video("Кліп 'Cyberpunk 2077'", 210, "1080p"),
                new Audio("Подкаст 'IT News'", 1800, 192)
            };

            foreach (var item in playlist)
            {
                item.Play();
            }

            Console.WriteLine("\n====================================================================");
            Console.WriteLine("  ПОЯСНЕННЯ РЕЗУЛЬТАТІВ:");
            Console.WriteLine("  1. 'Video' перевизначає (override) метод -> при виклику через Media");
            Console.WriteLine("     викликається версія Video (працює динамічне зв'язування/поліморфізм).");
            Console.WriteLine("  2. 'Audio' приховує (new) метод -> при виклику через Media викликається");
            Console.WriteLine("     базова версія Media, бо метод обирається за ТИПОМ ПОСИЛАННЯ (статичне зв'язування).");
            Console.WriteLine("====================================================================");
        }
    }
}