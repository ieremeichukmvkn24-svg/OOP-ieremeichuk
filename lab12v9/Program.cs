using System;

namespace lab12v9
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IRepository<Project> projectRepo = new InMemoryRepository<Project>();

            // 1. Додавання елементів
            projectRepo.Add(new Project(1, "Alpha System", new DateTime(2026, 1, 10), new DateTime(2026, 6, 30)));
            projectRepo.Add(new Project(2, "Beta Platform", new DateTime(2026, 3, 1), new DateTime(2026, 12, 31)));

            Console.WriteLine("=== Усі проєкти після додавання ===");
            foreach (var p in projectRepo.GetAll())
            {
                Console.WriteLine($"ID: {p.Id}, Назва: {p.Name}, Початок: {p.StartDate.ToShortDateString()}");
            }

            // 2. Оновлення елемента
            var projectToUpdate = projectRepo.GetById(1);
            if (projectToUpdate != null)
            {
                projectToUpdate.Name = "Alpha System v2.0";
                projectRepo.Update(projectToUpdate);
            }

            Console.WriteLine("\n=== Після оновлення проєкта з ID 1 ===");
            var updated = projectRepo.GetById(1);
            if (updated != null)
            {
                Console.WriteLine($"ID: {updated.Id}, Нова назва: {updated.Name}");
            }

            // 3. Видалення елемента
            projectRepo.Delete(2);
            Console.WriteLine("\n=== Після видалення проєкта з ID 2 ===");
            foreach (var p in projectRepo.GetAll())
            {
                Console.WriteLine($"ID: {p.Id}, Назва: {p.Name}");
            }
        }
    }
}