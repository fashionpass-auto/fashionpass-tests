using FashionPass.Tests.Config;
using Microsoft.Playwright;

namespace FashionPass.Tests.Utilities;

public static class ScreenshotHelper
{
    public static async Task CaptureAsync(IPage page, TestConfig config, string testName)
    {
        var directory = Path.Combine(AppContext.BaseDirectory, config.Screenshots.Directory);
        Directory.CreateDirectory(directory);

        var safeName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
        var path = Path.Combine(directory, $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

        await page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = path,
            FullPage = true
        });

        TestContext.AddTestAttachment(path);
    }
}