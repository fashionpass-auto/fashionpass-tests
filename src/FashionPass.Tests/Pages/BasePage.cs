using FashionPass.Tests.Config;
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
        await Page.GotoAsync(urlPath ?? UrlPath, new PageGotoOptions
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
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = Config.Timeouts.Default
        });
        await locator.ClickAsync();
    }

    protected async Task FillAsync(ILocator locator, string value)
    {
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = Config.Timeouts.Default
        });
        await locator.FillAsync(value);
    }

    protected async Task<bool> IsVisibleAsync(ILocator locator) => await locator.IsVisibleAsync();
}