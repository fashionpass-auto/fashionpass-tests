using FashionPass.Tests.Config;
using Microsoft.Playwright;

namespace FashionPass.Tests.Drivers;

public static class PlaywrightDriverFactory
{
    public static Task<IPlaywright> CreatePlaywrightAsync() => Playwright.CreateAsync();

    public static async Task<IBrowser> CreateBrowserAsync(IPlaywright playwright, TestConfig config)
    {
        var browserTypeName = config.Browser.Type.ToLowerInvariant();
        var browserType = browserTypeName switch
        {
            "chromium" or "chrome" => playwright.Chromium,
            "webkit" or "safari" => playwright.Webkit,
            "firefox" => playwright.Firefox,
            _ => throw new ArgumentOutOfRangeException(
                nameof(config.Browser.Type),
                browserTypeName,
                $"Unsupported browser type. Use 'chromium', 'webkit' or 'firefox'.")
        };

        return await browserType.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = config.Browser.Headless,
            Channel = config.Browser.Channel,
            Timeout = config.Timeouts.Navigation
        });
    }

    public static BrowserNewContextOptions BuildContextOptions(TestConfig config) => new()
    {
        BaseURL = config.BaseUrl,
        ViewportSize = new ViewportSize { Width = config.Browser.Width, Height = config.Browser.Height },
        Locale = config.Browser.Locale,
        TimezoneId = config.Browser.TimezoneId,
        IgnoreHTTPSErrors = config.Browser.IgnoreHttpsErrors
    };
}