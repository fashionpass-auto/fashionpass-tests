using FashionPass.Tests.Hooks;
using FashionPass.Tests.Pages.Web;
using FluentAssertions;

namespace FashionPass.Tests.Tests.Web;

public sealed class HomePageTests : WebTest
{
    [Test]
    public async Task HomePage_Loads_And_Displays_HeroSection()
    {
        var homePage = new HomePage(Page, Config);
        await homePage.GotoAsync();
        await homePage.WaitForHeroAsync();

        homePage.GetCurrentUrl().Should().Contain("fashionpass.com");
        (await homePage.IsJoinTheClubVisibleAsync()).Should().BeTrue();
    }

    [Test]
    public async Task HomePage_Has_Expected_Title()
    {
        var homePage = new HomePage(Page, Config);
        await homePage.GotoAsync();
        await homePage.WaitForHeroAsync();

        var title = await homePage.GetPageTitleAsync();
        title.Should().Contain("FashionPass");
    }

    [Test]
    public async Task HomePage_Header_Is_Present()
    {
        var homePage = new HomePage(Page, Config);
        await homePage.GotoAsync();
        await homePage.WaitForHeroAsync();

        (await homePage.Header.WaitUntilHydratedAsync()).Should().BeTrue();
        (await homePage.Header.IsBrowseLinkVisibleAsync()).Should().BeTrue();
    }
}