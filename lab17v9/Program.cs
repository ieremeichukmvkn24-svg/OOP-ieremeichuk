using System.Net.Http;

namespace lab17v9;

static class RetryPolicy
{
    public static async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> action,
        int maxAttempts,
        TimeSpan delay,
        Func<Exception, bool> isTransient,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(isTransient);
        if (maxAttempts < 1) throw new ArgumentOutOfRangeException(nameof(maxAttempts));
        if (delay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(delay));

        for (int attempt = 1; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try { return await action(cancellationToken); }
            catch (Exception ex) when (
                attempt < maxAttempts &&
                !cancellationToken.IsCancellationRequested &&
                isTransient(ex))
            {
                double waitMs = Math.Min(2000, delay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                Console.WriteLine("Спроба " + attempt + ": " + ex.Message +
                                  ". Retry за " + waitMs + " мс");
                await Task.Delay(TimeSpan.FromMilliseconds(waitMs), cancellationToken);
            }
        }
    }

    public static bool IsTransient(Exception ex) =>
        ex is HttpRequestException ||
        ex is TimeoutException ||
        (ex is IOException &&
         ex is not FileNotFoundException &&
         ex is not DirectoryNotFoundException);
}

sealed class SimulatedProjectApi
{
    private int failures;
    public int Calls { get; private set; }

    public SimulatedProjectApi(int initialFailures) => failures = initialFailures;

    public Task<string> GetStatusAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Calls++;
        if (failures-- > 0)
            throw new HttpRequestException("Тимчасове порушення мережевого зв'язку");
        return Task.FromResult("Project 9: active");
    }
}

sealed class SimulatedFileReader
{
    private int failures;

    public SimulatedFileReader(int initialFailures) => failures = initialFailures;

    public Task<string> ReadAsync(string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (failures-- > 0) throw new IOException("Файл тимчасово зайнятий");
        return File.ReadAllTextAsync(path, cancellationToken);
    }
}

static class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        try
        {
            if (args.Contains("--test"))
            {
                await SelfTest();
                Console.WriteLine("LAB17 SELF-TEST PASSED");
                return 0;
            }

            Console.WriteLine("Лаб 17 (варіант 9) — I/O та мережеві помилки");
            var api = new SimulatedProjectApi(2);
            string status = await RetryPolicy.ExecuteAsync(
                ct => api.GetStatusAsync(ct), 4, TimeSpan.FromMilliseconds(60),
                RetryPolicy.IsTransient);
            Console.WriteLine("Результат API: " + status + " (спроб: " + api.Calls + ")");

            string file = Path.GetTempFileName();
            try
            {
                await File.WriteAllTextAsync(file, "Project 9 — успішне читання");
                var reader = new SimulatedFileReader(1);
                string data = await RetryPolicy.ExecuteAsync(
                    ct => reader.ReadAsync(file, ct), 3, TimeSpan.FromMilliseconds(60),
                    RetryPolicy.IsTransient);
                Console.WriteLine("Файл: " + data);
            }
            finally { File.Delete(file); }

            try
            {
                await RetryPolicy.ExecuteAsync(
                    ct => File.ReadAllTextAsync(
                        Path.Combine(Path.GetTempPath(), "oop_lab17_v9_nonexistent_file.txt"), ct),
                    3, TimeSpan.FromMilliseconds(10), RetryPolicy.IsTransient);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Відсутній файл — постійна помилка, без повторів");
            }

            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    static async Task SelfTest()
    {
        var api = new SimulatedProjectApi(2);
        string result = await RetryPolicy.ExecuteAsync(
            ct => api.GetStatusAsync(ct), 3, TimeSpan.Zero, RetryPolicy.IsTransient);
        Check(result.Contains("active") && api.Calls == 3, "retries succeed");

        var unreachable = new SimulatedProjectApi(10);
        bool exhausted = false;
        try
        {
            await RetryPolicy.ExecuteAsync(
                ct => unreachable.GetStatusAsync(ct), 2, TimeSpan.Zero,
                RetryPolicy.IsTransient);
        }
        catch (HttpRequestException) { exhausted = true; }
        Check(exhausted && unreachable.Calls == 2, "attempt limit");

        int attempts = 0;
        bool failedFast = false;
        try
        {
            await RetryPolicy.ExecuteAsync<string>(
                _ => { attempts++; throw new InvalidOperationException("Permanent"); },
                3, TimeSpan.Zero, RetryPolicy.IsTransient);
        }
        catch (InvalidOperationException) { failedFast = true; }
        Check(failedFast && attempts == 1, "permanent error");

        Check(!RetryPolicy.IsTransient(new FileNotFoundException()), "missing file exclusion");
        Check(RetryPolicy.IsTransient(new IOException()), "retry temporary I/O");
    }

    static void Check(bool ok, string description)
    {
        if (!ok) throw new Exception("Self-test failed: " + description);
    }
}
