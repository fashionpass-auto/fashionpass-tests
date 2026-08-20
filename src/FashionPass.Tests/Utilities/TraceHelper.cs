using FashionPass.Tests.Config;
using Microsoft.Playwright;

namespace FashionPass.Tests.Utilities;

public static class TraceHelper
{
    public static async Task StartAsync(IBrowserContext context)
    {
        await context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = false,
            Sources = false
        });
    }

    public static async Task<string> StopAndSaveAsync(IBrowserContext context, TestConfig config, string testName)
    {
        var directory = Path.Combine(AppContext.BaseDirectory, config.Screenshots.Directory, "traces");
        Directory.CreateDirectory(directory);

        var safeName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
        var path = Path.Combine(directory, $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}.zip");

        await context.Tracing.StopAsync(new TracingStopOptions { Path = path });

        var sanitizedPath = TraceSanitizer.Sanitize(path, config);

        TestContext.AddTestAttachment(sanitizedPath);
        return sanitizedPath;
    }

    public static Task StopDiscardAsync(IBrowserContext context) => context.Tracing.StopAsync();
}