using FashionPass.Tests.Config;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Components;

public sealed class AccountComponent
{
    private readonly IPage _page;
    private readonly TestConfig _config;

    public AccountComponent(IPage page, TestConfig config)
    {
        _page = page;
        _config = config;
    }

    public async Task ClickAccountButtonAsync(string userName)
    {
        var button = _page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = userName });
        await button.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _config.Timeouts.Default
        });
        await button.ClickAsync();
    }

    public async Task<bool> IsGreetingVisibleAsync(string userName)
    {
        try
        {
            var greeting = _page.GetByText(string.Format(Selectors.Account.GreetingFormat, userName),
                new PageGetByTextOptions { Exact = true });
            await greeting.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = _config.Timeouts.Default
            });
            return true;
        }
        catch (PlaywrightException)
        {
            return false;
        }
    }

    public async Task CloseMenuAsync()
    {
        var backdrop = _page.Locator(Selectors.Account.Backdrop);
        await backdrop.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _config.Timeouts.Default
        });
        await backdrop.ClickAsync();
    }

    public async Task<bool> IsAccountButtonVisibleAsync(string userName)
        => await WaitForVisibleAsync(_page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = userName }));

    private async Task<bool> WaitForVisibleAsync(ILocator locator)
    {
        try
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = _config.Timeouts.Default
            });
            return true;
        }
        catch (PlaywrightException)
        {
            return false;
        }
    }
}