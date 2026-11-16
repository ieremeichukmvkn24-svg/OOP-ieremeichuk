namespace lab16v9;

record WorkItem(int Id, string Title, string Assignee, bool Done, string[] Tags);
record Project(int Id, string Name, IReadOnlyList<WorkItem> Tasks);
record ProjectTask(string ProjectName, WorkItem Task);
record Page<T>(IReadOnlyList<T> Items, int Total, int Number, int Size)
{
    public int TotalPages => (int)Math.Ceiling((double)Total / Size);
}

static class QueryExtensions
{
    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source, bool enabled, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(predicate);
        return enabled ? source.Where(predicate) : source;
    }

    public static Page<T> Paginate<T>(this IEnumerable<T> source, int number, int size)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (number <= 0) throw new ArgumentOutOfRangeException(nameof(number));
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
        var all = source.ToList();
        long skip = (long)(number - 1) * size;
        var items = skip >= all.Count
            ? new List<T>()
            : all.Skip((int)skip).Take(size).ToList();
        return new Page<T>(items, all.Count, number, size);
    }

    public static IEnumerable<T> DistinctByKey<T, TKey>(
        this IEnumerable<T> source, Func<T, TKey> selector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);
        var seen = new HashSet<TKey>();
        foreach (var item in source)
            if (seen.Add(selector(item)))
                yield return item;
    }
}

static class Program
{
    static readonly Project[] Projects =
    {
        new(1, "Marketplace", new[]
        {
            new WorkItem(1, "Каталог", "Максим", false, new[] { "backend", "web" }),
            new WorkItem(2, "Кошик", "Олена", true, new[] { "web" }),
            new WorkItem(3, "Оплата", "Максим", false, new[] { "backend", "security" })
        }),
        new(2, "Portfolio", new[]
        {
            new WorkItem(4, "Верстка", "Андрій", true, new[] { "frontend", "web" }),
            new WorkItem(5, "Галерея", "Олена", false, new[] { "frontend" })
        })
    };

    static List<ProjectTask> Flatten() =>
        Projects.SelectMany(project => project.Tasks,
            (project, task) => new ProjectTask(project.Name, task)).ToList();

    static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        try
        {
            var tasks = Flatten();
            if (args.Contains("--test"))
            {
                Check(tasks.Count == 5, "flatten");
                Check(tasks.SelectMany(x => x.Task.Tags).Distinct().Count() == 4, "tags");
                Check(tasks.WhereIf(true, x => !x.Task.Done).Count() == 3, "where-if");
                Check(tasks.WhereIf(false, x => !x.Task.Done).Count() == 5, "where-if off");
                var page = tasks.OrderBy(x => x.Task.Id).Paginate(2, 2);
                Check(page.Total == 5 && page.TotalPages == 3 && page.Items.Count == 2 &&
                      page.Items[0].Task.Id == 3, "pagination");
                Check(tasks.Paginate(10, 2).Items.Count == 0, "out-of-range page");
                Check(tasks.DistinctByKey(x => x.Task.Assignee).Count() == 3, "distinct");
                bool invalid = false;
                try { _ = tasks.Paginate(0, 3); }
                catch (ArgumentOutOfRangeException) { invalid = true; }
                Check(invalid, "validation");
                Console.WriteLine("LAB16 SELF-TEST PASSED");
                return 0;
            }

            Console.WriteLine("Лаб 16 (варіант 9) — SelectMany і пагінація");
            foreach (var x in tasks)
                Console.WriteLine(x.ProjectName + " -> " + x.Task.Title);

            Console.WriteLine("\nУнікальні теги:");
            foreach (var tag in tasks.SelectMany(x => x.Task.Tags).Distinct().OrderBy(x => x))
                Console.WriteLine(tag);

            Console.WriteLine("\nНезавершені задачі:");
            foreach (var x in tasks.WhereIf(true, x => !x.Task.Done))
                Console.WriteLine(x.Task.Title);

            var result = tasks.OrderBy(x => x.Task.Id).Paginate(2, 2);
            Console.WriteLine("\nСторінка " + result.Number + "/" + result.TotalPages);
            foreach (var x in result.Items) Console.WriteLine(x.Task.Title);

            Console.WriteLine("\nОдин запис на кожного виконавця:");
            foreach (var x in tasks.DistinctByKey(x => x.Task.Assignee))
                Console.WriteLine(x.Task.Assignee + ": " + x.Task.Title);
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    static void Check(bool condition, string name)
    {
        if (!condition) throw new Exception("Self-test failed: " + name);
    }
}
