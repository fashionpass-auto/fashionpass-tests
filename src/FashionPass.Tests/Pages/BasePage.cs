using FashionPass.Tests.Config;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly TestConfig Config;

    protected BasePage(IPage page, TestConfig config)
    {
        Page = page;
        Config = config;
    }

    public abstract string UrlPath { get; }

    public virtual async Task GotoAsync(string? urlPath = null)
    {
        var target = urlPath ?? UrlPath;
        TestActivityCollector.Current?.RecordAction($"Navigate to {target}");
        await Page.GotoAsync(target, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.Load,
            Timeout = Config.Timeouts.Navigation
        });
    }

    public Task<string> GetTitleAsync() => Page.TitleAsync();

    public string GetCurrentUrl() => Page.Url;

    protected ILocator Locator(string selector) => Page.Locator(selector).First;

    protected ILocator GetByRole(AriaRole role, string name, bool exact = true)
        => Page.GetByRole(role, new PageGetByRoleOptions { Name = name, Exact = exact });

    protected async Task ClickAsync(ILocator locator)
    {
        TestActivityCollector.Current?.RecordAction($"Click on {locator}");
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = Config.Timeouts.Default
        });
        await locator.ClickAsync();
    }

    protected async Task FillAsync(ILocator locator, string value)
    {
        TestActivityCollector.Current?.RecordAction($"Fill '{value}' into {locator}");
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = Config.Timeouts.Default
        });
        await locator.FillAsync(value);
    }

    protected async Task<bool> IsVisibleAsync(ILocator locator) => await locator.IsVisibleAsync();

    private static readonly string[] IgnoredTrackingHosts =
{
    "trkn.us", "google-analytics.com", "googletagmanager.com",
    "facebook.net", "doubleclick.net", "hotjar.com", "mixpanel.com",
    "segment.io", "scorecardresearch.com", "quantserve.com",
    "criteo.com", "taboola.com", "outbrain.com", "bat.bing.com"
};

public async Task WaitForImagesToLoadAsync(int timeoutMs = 15000)
{
    try
    {
        await Page.WaitForFunctionAsync(
            "() => Array.from(document.querySelectorAll('img')).every(i => i.complete)",
            null,
            new PageWaitForFunctionOptions { Timeout = timeoutMs });
    }
    catch (Exception)
    {
        // Lazy-loaded images may never complete until scrolled; fall through to the check.
    }
}

public async Task<string[]> GetBrokenImagesAsync()
{
    var broken = await Page.EvaluateAsync<string[]>(
        "() => Array.from(document.querySelectorAll('img'))" +
        ".filter(i => i.complete && i.naturalWidth === 0).map(i => i.src)");

return broken
        .Where(src => !IgnoredTrackingHosts.Any(host =>
            src.Contains(host, StringComparison.OrdinalIgnoreCase)))
        .ToArray();
    }

public async Task<string[]> GetBrokenIconsAsync()
    => await Page.EvaluateAsync<string[]>(
        "() => Array.from(document.querySelectorAll('svg use'))" +
        ".filter(u => !u.getAttribute('href') && !u.getAttribute('xlink:href'))" +
        ".map(u => u.outerHTML.slice(0, 120))");

    public IReadOnlyList<string> GetRelevantConsoleErrors()
        => (TestActivityCollector.Current?.ConsoleErrors ?? Array.Empty<string>())
            .Where(msg => !IgnoredTrackingHosts.Any(host =>
                msg.Contains(host, StringComparison.OrdinalIgnoreCase)))
            .ToList();
}