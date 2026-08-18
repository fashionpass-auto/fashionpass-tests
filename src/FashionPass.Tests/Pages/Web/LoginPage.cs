using FashionPass.Tests.Config;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Web;

public sealed class LoginPage : BasePage
{
    public LoginPage(IPage page, TestConfig config) : base(page, config)
    {
    }

    public override string UrlPath => "/login";

    public async Task LoginAsync(string email, string password)
    {
        await FillAsync(Locator(Selectors.Login.EmailInput), email);
        await FillAsync(Locator(Selectors.Login.PasswordInput), password);
        await ClickAsync(Locator(Selectors.Login.SubmitButton));
    }

    public async Task<bool> IsEmailInputVisibleAsync()
        => await IsVisibleAsync(Locator(Selectors.Login.EmailInput));

    public async Task<bool> IsErrorMessageVisibleAsync()
        => await IsVisibleAsync(Locator(Selectors.Login.ErrorMessage));
}