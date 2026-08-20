using FashionPass.Tests.Hooks;
using FashionPass.Tests.Pages.Web;
using FluentAssertions;

namespace FashionPass.Tests.Tests.Web;

public sealed class ImageLoadTests : WebTest
{
    [Test]
    public async Task HomePage_All_Images_Load_Before_Login()
    {
        var home = new HomePage(Page, Config);
        await home.GotoAsync();
        await home.WaitForPromoPopupAndDismissAsync();

        await home.WaitForImagesToLoadAsync();

        var brokenImages = await home.GetBrokenImagesAsync();
        brokenImages.Should().BeEmpty();
    }

    [Test]
    public async Task MainPage_Loads_And_Images_Load_After_Login()
    {
        // 1. Go to fashionpass.com
        var home = new HomePage(Page, Config);
        await home.GotoAsync();

        // 2. Wait for the modal popup to appear, then close it
        (await home.WaitForPromoPopupAndDismissAsync()).Should().BeTrue();

        // 3. Sign in with the configured account
        var login = await home.ClickSignInAsync();
        await login.LoginAsync(Config.Users.Default.Email, Config.Users.Default.Password);
        (await home.Header.WaitUntilSignedInAsync()).Should().BeTrue();

        // 4. After login, go to the Main page again
        await home.GotoAsync();
        await home.WaitForPromoPopupAndDismissAsync();

        // 5. Page loads successfully
        (await home.GetPageTitleAsync()).Should().NotBeNullOrWhiteSpace();
        home.GetRelevantConsoleErrors().Should().BeEmpty();
        Activity.FailedResponses.Should().BeEmpty();

        // 6. All images and icons load
        await home.WaitForImagesToLoadAsync();

        var brokenImages = await home.GetBrokenImagesAsync();
        brokenImages.Should().BeEmpty();

        var brokenIcons = await home.GetBrokenIconsAsync();
        brokenIcons.Should().BeEmpty();
    }
}