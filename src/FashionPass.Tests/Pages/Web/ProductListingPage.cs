using FashionPass.Tests.Config;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Web;

public sealed class ProductListingPage : BasePage
{
    public ProductListingPage(IPage page, TestConfig config) : base(page, config)
    {
    }

    public override string UrlPath => "/clothing";

    public async Task<string> GetCategoryTitleAsync()
        => await Page.Locator(Selectors.ProductListing.CategoryTitle).InnerTextAsync();

    public async Task<int> GetProductCountAsync()
        => await Page.Locator(Selectors.ProductListing.ProductLink).CountAsync();

    public async Task<ProductDetailPage> OpenFirstProductAsync()
    {
        await ClickAsync(Page.Locator(Selectors.ProductListing.ProductLink).First);
        return new ProductDetailPage(Page, Config);
    }
}