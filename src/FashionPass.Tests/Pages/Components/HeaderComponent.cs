using FashionPass.Tests.Config;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Components;

public sealed class HeaderComponent
{
    private readonly IPage _page;
    private readonly TestConfig _config;
    private readonly ILocator _searchInput;
    private readonly ILocator _loginLink;
    private readonly ILocator _bagLink;

    public HeaderComponent(IPage page, TestConfig config)
    {
        _page = page;
        _config = config;
        _searchInput = page.Locator(Utilities.Selectors.Header.SearchInput).First;
        _loginLink = page.Locator(Utilities.Selectors.Header.LoginLink).First;
        _bagLink = page.Locator(Utilities.Selectors.Header.BagLink).First;
    }

    public async Task SearchAsync(string term)
    {
        await _searchInput.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _config.Timeouts.Default
        });
        await _searchInput.FillAsync(term);
        await _searchInput.PressAsync("Enter");
    }

    public async Task<bool> WaitUntilHydratedAsync()
    {
        try
        {
            await _page.Locator(Utilities.Selectors.Nav.BrowseLink).First.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Attached,
                Timeout = _config.Timeouts.Default
            });
            return true;
        }
        catch (PlaywrightException)
        {
            return false;
        }
    }

    public async Task<bool> IsBrowseLinkVisibleAsync()
        => await _page.Locator(Utilities.Selectors.Nav.BrowseLink).IsVisibleAsync();

    public async Task ClickSignInAsync()
    {
        var signIn = _page.GetByText(Utilities.Selectors.Login.SignInTriggerText).First;
        await signIn.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _config.Timeouts.Default
        });
        await signIn.ClickAsync();
    }

    public async Task<bool> IsLoginLinkVisibleAsync() => await _loginLink.IsVisibleAsync();

    public async Task<bool> IsBagLinkVisibleAsync() => await _bagLink.IsVisibleAsync();
}