namespace lab18v9;

record Project(int Id, string Name, DateTime StartDate, DateTime EndDate, decimal Budget);
record DomainError(string Code, string Message);

sealed class Result<T> where T : class
{
    private readonly T? value;
    public bool IsSuccess { get; }
    public DomainError? Error { get; }

    public T Value => IsSuccess
        ? value!
        : throw new InvalidOperationException("Неуспішний Result не має значення.");

    private Result(T? value, DomainError? error)
    {
        this.value = value;
        Error = error;
        IsSuccess = error is null;
    }

    public static Result<T> Ok(T value) =>
        new(value ?? throw new ArgumentNullException(nameof(value)), null);

    public static Result<T> Fail(string code, string message) =>
        new(null, new DomainError(code, message));
}

abstract class ProjectDomainException : Exception
{
    public string Code { get; }
    protected ProjectDomainException(string code, string message) : base(message) =>
        Code = code;
}

sealed class ProjectValidationException : ProjectDomainException
{
    public ProjectValidationException(string message) : base("VALIDATION", message) { }
}

sealed class ProjectConflictException : ProjectDomainException
{
    public ProjectConflictException(int id) :
        base("CONFLICT", "Проєкт із ID " + id + " вже існує.") { }
}

sealed class ProjectNotFoundException : ProjectDomainException
{
    public ProjectNotFoundException(int id) :
        base("NOT_FOUND", "Проєкт із ID " + id + " не знайдено.") { }
}

sealed class ProjectService
{
    private readonly Dictionary<int, Project> projects = new();

    public Project CreateStrict(Project project)
    {
        Validate(project);
        if (!projects.TryAdd(project.Id, project))
            throw new ProjectConflictException(project.Id);
        return project;
    }

    public Project GetStrict(int id) =>
        projects.TryGetValue(id, out var project)
            ? project
            : throw new ProjectNotFoundException(id);

    public Project UpdateStrict(Project project)
    {
        Validate(project);
        if (!projects.ContainsKey(project.Id))
            throw new ProjectNotFoundException(project.Id);
        projects[project.Id] = project;
        return project;
    }

    public Result<Project> TryCreate(Project project) => Wrap(() => CreateStrict(project));
    public Result<Project> TryGet(int id) => Wrap(() => GetStrict(id));
    public Result<Project> TryUpdate(Project project) => Wrap(() => UpdateStrict(project));

    static Result<Project> Wrap(Func<Project> action)
    {
        try { return Result<Project>.Ok(action()); }
        catch (ProjectDomainException ex)
        {
            return Result<Project>.Fail(ex.Code, ex.Message);
        }
    }

    static void Validate(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        if (project.Id <= 0)
            throw new ProjectValidationException("ID має бути додатним.");
        if (string.IsNullOrWhiteSpace(project.Name))
            throw new ProjectValidationException("Потрібна назва проєкту.");
        if (project.EndDate < project.StartDate)
            throw new ProjectValidationException("Дата завершення раніше за початок.");
        if (project.Budget <= 0)
            throw new ProjectValidationException("Бюджет має бути додатним.");
    }
}

static class Program
{
    static Project Sample => new(9, "Marketplace", new DateTime(2026, 9, 1),
                                 new DateTime(2026, 12, 31), 120_000m);

    static int Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        try
        {
            if (args.Contains("--test"))
            {
                SelfTest();
                Console.WriteLine("LAB18 SELF-TEST PASSED");
                return 0;
            }

            Console.WriteLine("Лаб 18 (варіант 9) — власні винятки та Result");
            var service = new ProjectService();
            Print("Створення", service.TryCreate(Sample));
            Print("Повторне створення", service.TryCreate(Sample));
            Print("Пошук ID 100", service.TryGet(100));
            Print("Порушення дат", service.TryCreate(Sample with
            {
                Id = 10, EndDate = new DateTime(2026, 1, 1)
            }));
            Print("Оновлення", service.TryUpdate(Sample with { Budget = 140_000m }));

            try { service.GetStrict(999); }
            catch (ProjectNotFoundException ex)
            {
                Console.WriteLine("Власний виняток: " + ex.Code + " — " + ex.Message);
            }
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    static void Print(string label, Result<Project> result)
    {
        Console.WriteLine(label + ": " + (result.IsSuccess
            ? "OK — " + result.Value.Name + ", " + result.Value.Budget + " грн"
            : result.Error!.Code + " — " + result.Error.Message));
    }

    static void SelfTest()
    {
        var service = new ProjectService();
        Check(service.TryCreate(Sample).IsSuccess, "create");
        Check(service.TryCreate(Sample).Error?.Code == "CONFLICT", "duplicate");
        Check(service.TryGet(100).Error?.Code == "NOT_FOUND", "not found");
        var invalid = service.TryCreate(Sample with { Id = 10, Name = "" });
        Check(invalid.Error?.Code == "VALIDATION", "name validation");
        Check(service.TryCreate(Sample with
        {
            Id = 10, EndDate = Sample.StartDate.AddDays(-1)
        }).Error?.Code == "VALIDATION", "date validation");
        Check(service.TryUpdate(Sample with { Budget = 200_000m }).IsSuccess, "update");
        Check(service.TryGet(9).Value.Budget == 200_000m, "get after update");

        bool caught = false;
        try { service.GetStrict(404); }
        catch (ProjectNotFoundException) { caught = true; }
        Check(caught, "custom exception");

        bool rejected = false;
        try { _ = invalid.Value; }
        catch (InvalidOperationException) { rejected = true; }
        Check(rejected, "failed result value");
    }

    static void Check(bool ok, string name)
    {
        if (!ok) throw new Exception("Self-test failed: " + name);
    }
}
