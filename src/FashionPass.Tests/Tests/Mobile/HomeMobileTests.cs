using FashionPass.Tests.Hooks;
using FashionPass.Tests.Pages.Mobile;
using FluentAssertions;

namespace FashionPass.Tests.Tests.Mobile;

public sealed class HomeMobileTests : MobileTest
{
    [Test]
    public void HomeScreen_Is_Displayed()
    {
        var homePage = new HomeMobilePage(Driver, Config);

        homePage.IsHomeScreenVisible().Should().BeTrue();
    }
}