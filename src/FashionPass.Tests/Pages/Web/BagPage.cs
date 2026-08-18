using FashionPass.Tests.Config;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Web;

public sealed class BagPage : BasePage
{
    public BagPage(IPage page, TestConfig config) : base(page, config)
    {
    }

    public override string UrlPath => "/bag";

    public async Task<string> GetItemCountAsync()
        => await Page.Locator(Selectors.Bag.ItemCount).InnerTextAsync();

    public async Task<bool> IsCheckoutButtonVisibleAsync()
        => await IsVisibleAsync(Locator(Selectors.Bag.CheckoutButton));
}