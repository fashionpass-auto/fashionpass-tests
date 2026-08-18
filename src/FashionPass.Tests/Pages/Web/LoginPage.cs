using FashionPass.Tests.Config;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Web;

public sealed class LoginPage : BasePage
{
    public LoginPage(IPage page, TestConfig config) : base(page, config)
    {
    }

    public override string UrlPath => "/";

    public async Task LoginAsync(string email, string? password = null)
    {
        await FillAsync(GetByRole(AriaRole.Textbox, Selectors.Login.EmailFieldName), email);

        if (!string.IsNullOrEmpty(password))
        {
            var passwordField = Locator(Selectors.Login.PasswordInput);
            if (await passwordField.IsVisibleAsync())
                await FillAsync(passwordField, password);
        }

        await ClickAsync(GetByRole(AriaRole.Button, Selectors.Login.LogInButtonName));
    }

    public async Task<bool> IsEmailFieldVisibleAsync()
        => await IsVisibleAsync(GetByRole(AriaRole.Textbox, Selectors.Login.EmailFieldName));
}