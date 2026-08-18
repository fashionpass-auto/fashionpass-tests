using FashionPass.Tests.Config;
using FashionPass.Tests.Pages.Components;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Web;

public sealed class HomePage : BasePage
{
    public HeaderComponent Header { get; }
    public FooterComponent Footer { get; }
    public AccountComponent Account { get; }

    public HomePage(IPage page, TestConfig config) : base(page, config)
    {
        Header = new HeaderComponent(page, config);
        Footer = new FooterComponent(page, config);
        Account = new AccountComponent(page, config);
    }

    public override string UrlPath => "/";

    public async Task WaitForHeroAsync()
    {
        await Page.Locator(Selectors.Home.HeroSection).WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = Config.Timeouts.Default
        });
    }

    public async Task<bool> IsJoinTheClubVisibleAsync()
        => await IsVisibleAsync(Locator(Selectors.Home.JoinTheClubButton));

    public async Task<bool> DismissPromoPopupIfPresentAsync()
    {
        try
        {
            var dismiss = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
            {
                Name = Selectors.Promo.DismissButtonText,
                Exact = false
            });
            await dismiss.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = Selectors.Promo.PopupWaitMs
            });
            await dismiss.ClickAsync();
            return true;
        }
        catch (PlaywrightException)
        {
            return false;
        }
    }

    public async Task<LoginPage> ClickSignInAsync()
    {
        await Header.ClickSignInAsync();
        return new LoginPage(Page, Config);
    }

    public async Task<SignUpPage> ClickJoinTheClubAsync()
    {
        await ClickAsync(Locator(Selectors.Home.JoinTheClubButton));
        return new SignUpPage(Page, Config);
    }

    public async Task<string> GetPageTitleAsync() => await GetTitleAsync();
}