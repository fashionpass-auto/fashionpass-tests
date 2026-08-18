using FashionPass.Tests.Hooks;
using FashionPass.Tests.Pages.Web;
using FashionPass.Tests.Utilities;
using FluentAssertions;
using Microsoft.Playwright;

namespace FashionPass.Tests.Tests.Web;

public sealed class NavigationTests : WebTest
{
    [TestCase("/clothing", "Browse")]
    [TestCase("/occasions", "Occasions")]
    [TestCase("/how-it-works", "How It Works")]
    public async Task HomePage_NavigationLinks_NavigateToExpectedPage(string expectedPath, string linkText)
    {
        var homePage = new HomePage(Page, Config);
        await homePage.GotoAsync();
        await homePage.WaitForHeroAsync();

        await Page.Locator($"a[href='{expectedPath}']").ClickAsync();

        await Page.WaitForURLAsync($"**{expectedPath}**", new PageWaitForURLOptions
        {
            Timeout = Config.Timeouts.Navigation
        });
    }

    [Test]
    public async Task HomePage_JoinTheClub_OpensSignUp()
    {
        var homePage = new HomePage(Page, Config);
        await homePage.GotoAsync();
        await homePage.WaitForHeroAsync();

        await homePage.ClickJoinTheClubAsync();

        await Page.WaitForURLAsync("**signup**", new PageWaitForURLOptions
        {
            Timeout = Config.Timeouts.Navigation
        });
    }

    [Test]
    public async Task ProductListing_Page_Loads_Products()
    {
        var listingPage = new ProductListingPage(Page, Config);
        await listingPage.GotoAsync("/clothing");

        var title = await listingPage.GetCategoryTitleAsync();
        var count = await listingPage.GetProductCountAsync();

        title.Should().NotBeNullOrWhiteSpace();
        count.Should().BeGreaterThan(0);
    }
}