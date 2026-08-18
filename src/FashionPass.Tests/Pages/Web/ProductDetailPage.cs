using FashionPass.Tests.Config;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Web;

public sealed class ProductDetailPage : BasePage
{
    public ProductDetailPage(IPage page, TestConfig config) : base(page, config)
    {
    }

    public override string UrlPath => string.Empty;

    public async Task<string> GetProductNameAsync()
        => await Page.Locator(Selectors.ProductDetail.ProductName).InnerTextAsync();

    public async Task SelectSizeAsync(string size)
    {
        var sizeControl = Page.Locator(Selectors.ProductDetail.SizeSelector);
        await sizeControl.GetByText(size, new LocatorGetByTextOptions { Exact = true }).First.ClickAsync();
    }

    public async Task AddToBagAsync()
    {
        var button = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to Bag" });
        if (await button.CountAsync() == 0)
            button = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Add to bag" });
        await ClickAsync(button.First);
    }
}