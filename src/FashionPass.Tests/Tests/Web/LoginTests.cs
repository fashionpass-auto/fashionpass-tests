using FashionPass.Tests.Hooks;
using FashionPass.Tests.Pages.Web;
using FluentAssertions;

namespace FashionPass.Tests.Tests.Web;

public sealed class LoginTests : WebTest
{
    [Test]
    public async Task Login_Displays_Email_Field()
    {
        var home = new HomePage(Page, Config);
        await home.GotoAsync();
        await home.DismissPromoPopupIfPresentAsync();

        var login = await home.ClickSignInAsync();

        (await login.IsEmailFieldVisibleAsync()).Should().BeTrue();
    }

    [Test]
    public async Task Login_With_Valid_Account_Shows_Greeting_And_Account_Menu()
    {
        var home = new HomePage(Page, Config);
        await home.GotoAsync();
        await home.DismissPromoPopupIfPresentAsync();

        var login = await home.ClickSignInAsync();
        await login.LoginAsync(Config.Users.Default.Email, Config.Users.Default.Password);

        await home.Account.ClickAccountButtonAsync(Config.Users.Default.FirstName);
        (await home.Account.IsGreetingVisibleAsync(Config.Users.Default.FirstName)).Should().BeTrue();

        await home.Account.CloseMenuAsync();
        (await home.Account.IsAccountButtonVisibleAsync(Config.Users.Default.FirstName)).Should().BeTrue();
    }
}