using System.Diagnostics;

namespace lab15v9;

record Project(int Id, string Name, int OwnerId, decimal Budget, bool Active);
record Owner(int Id, string Name);
record WorkLog(int ProjectId, int Hours);
record ProjectSummary(string Name, string Owner, int Hours, decimal Budget);

static class Analytics
{
    public static List<Project> Filter(IEnumerable<Project> source, decimal budget) =>
        source.Where(p => p.Active && p.Budget >= budget)
              .OrderByDescending(p => p.Budget).ToList();

    public static List<ProjectSummary> Reports(
        IEnumerable<Project> projects, IEnumerable<Owner> owners, IEnumerable<WorkLog> logs) =>
        (from p in projects
         join o in owners on p.OwnerId equals o.Id
         join log in logs on p.Id equals log.ProjectId into groupLogs
         select new ProjectSummary(p.Name, o.Name, groupLogs.Sum(x => x.Hours), p.Budget))
        .OrderByDescending(x => x.Hours).ToList();

    public static List<(string Owner, int Count, decimal Budget)> ByOwner(
        IEnumerable<Project> projects, IEnumerable<Owner> owners) =>
        (from p in projects
         join o in owners on p.OwnerId equals o.Id
         group p by o.Name into g
         orderby g.Key
         select new { Name = g.Key, Count = g.Count(), Budget = g.Sum(x => x.Budget) })
        .Select(x => (x.Name, x.Count, x.Budget)).ToList();

    public static (int Count, long LinqTicks, long LoopTicks) Compare(Project[] data)
    {
        _ = data.Count(x => x.Active); // прогрів
        var sw = Stopwatch.StartNew();
        int viaLinq = data.Count(x => x.Active && x.Budget > 100_000m);
        sw.Stop();
        long linqTicks = sw.ElapsedTicks;
        sw.Restart();
        int viaLoop = 0;
        foreach (var item in data)
            if (item.Active && item.Budget > 100_000m) viaLoop++;
        sw.Stop();
        if (viaLinq != viaLoop) throw new Exception("Different query results");
        return (viaLinq, linqTicks, sw.ElapsedTicks);
    }
}

static class Program
{
    static readonly Project[] Projects =
    {
        new(1, "Marketplace", 1, 150_000m, true),
        new(2, "Portfolio", 2, 40_000m, true),
        new(3, "Legacy API", 1, 25_000m, false)
    };
    static readonly Owner[] Owners = { new(1, "Олена"), new(2, "Андрій") };
    static readonly WorkLog[] Logs = { new(1, 12), new(1, 8), new(2, 5) };

    static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        try
        {
            var filter = Analytics.Filter(Projects, 30_000m);
            var reports = Analytics.Reports(Projects, Owners, Logs);
            var groups = Analytics.ByOwner(Projects, Owners);
            if (args.Contains("--test"))
            {
                Check(filter.Count == 2 && filter[0].Id == 1);
                Check(reports.Count == 3 && reports.Single(x => x.Name == "Marketplace").Hours == 20);
                Check(groups.Single(x => x.Owner == "Олена").Budget == 175_000m);
                Check(Analytics.Compare(Projects).Count == 1);
                Console.WriteLine("LAB15 SELF-TEST PASSED");
                return 0;
            }

            Console.WriteLine("Лаб 15, варіант 9 — LINQ");
            Console.WriteLine("Фільтрація: " + string.Join(", ", filter.Select(p => p.Name)));
            foreach (var report in reports)
                Console.WriteLine("JOIN: " + report.Name + " | " + report.Owner + " | " +
                                  report.Hours + " год | " + report.Budget + " грн");
            foreach (var group in groups)
                Console.WriteLine("GROUP BY: " + group.Owner + " — " + group.Count +
                                  " проєкт(и), " + group.Budget + " грн");

            var large = Enumerable.Range(1, 100_000)
                .Select(i => new Project(i, "Project " + i, 1, i * 5m, i % 3 != 0)).ToArray();
            var perf = Analytics.Compare(large);
            Console.WriteLine("100 000 записів. LINQ ticks: " + perf.LinqTicks +
                              "; foreach ticks: " + perf.LoopTicks +
                              "; результат: " + perf.Count);
            Console.WriteLine("Одне вимірювання не є повноцінним бенчмарком.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    static void Check(bool ok)
    {
        if (!ok) throw new Exception("Failed self-test");
    }
}
