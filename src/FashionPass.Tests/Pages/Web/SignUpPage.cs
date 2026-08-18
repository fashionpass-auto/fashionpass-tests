using FashionPass.Tests.Config;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Web;

public sealed class SignUpPage : BasePage
{
    public SignUpPage(IPage page, TestConfig config) : base(page, config)
    {
    }

    public override string UrlPath => "/signup";

    public async Task RegisterAsync(string firstName, string lastName, string email, string phone, string password)
    {
        await FillAsync(Locator(Selectors.SignUp.FirstNameInput), firstName);
        await FillAsync(Locator(Selectors.SignUp.LastNameInput), lastName);
        await FillAsync(Locator(Selectors.SignUp.EmailInput), email);
        await FillAsync(Locator(Selectors.SignUp.PhoneInput), phone);
        await FillAsync(Locator(Selectors.SignUp.PasswordInput), password);
        await ClickAsync(Locator(Selectors.SignUp.SubmitButton));
    }

    public async Task<bool> IsEmailInputVisibleAsync()
        => await IsVisibleAsync(Locator(Selectors.SignUp.EmailInput));
}