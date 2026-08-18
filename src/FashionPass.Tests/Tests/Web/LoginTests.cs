using FashionPass.Tests.Hooks;
using FashionPass.Tests.Pages.Web;
using FluentAssertions;

namespace FashionPass.Tests.Tests.Web;

public sealed class LoginTests : WebTest
{
    [Test]
    public async Task LoginPage_Displays_Email_And_Password_Fields()
    {
        var loginPage = new LoginPage(Page, Config);
        await loginPage.GotoAsync();

        (await loginPage.IsEmailInputVisibleAsync()).Should().BeTrue();
    }

    [Test]
    public async Task Login_With_Invalid_Credentials_Shows_Error()
    {
        var loginPage = new LoginPage(Page, Config);
        await loginPage.GotoAsync();

        await loginPage.LoginAsync("invalid-user@fashionpass.test", "wrong-password");

        (await loginPage.IsErrorMessageVisibleAsync()).Should().BeTrue();
    }
}