using FashionPass.Tests.Config;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Components;

public sealed class FooterComponent
{
    private readonly IPage _page;
    private readonly TestConfig _config;

    public FooterComponent(IPage page, TestConfig config)
    {
        _page = page;
        _config = config;
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator(Utilities.Selectors.Footer.Container)
            .ScrollIntoViewIfNeededAsync(new LocatorScrollIntoViewIfNeededOptions { Timeout = _config.Timeouts.Default });
    }

    public async Task<bool> IsAboutUsLinkVisibleAsync()
        => await _page.Locator(Utilities.Selectors.Footer.AboutUsLink).IsVisibleAsync();

    public async Task<bool> IsFaqLinkVisibleAsync()
        => await _page.Locator(Utilities.Selectors.Footer.FaqLink).IsVisibleAsync();
}