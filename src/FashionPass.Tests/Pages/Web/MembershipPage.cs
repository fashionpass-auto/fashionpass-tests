using FashionPass.Tests.Config;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;

namespace FashionPass.Tests.Pages.Web;

public sealed class MembershipPage : BasePage
{
    public MembershipPage(IPage page, TestConfig config) : base(page, config)
    {
    }

    public override string UrlPath => "/signup";

    public async Task<List<string>> GetVisiblePlanNamesAsync()
    {
        var cards = Page.Locator(Selectors.Plans.PlanCard);
        var count = await cards.CountAsync();
        var names = new List<string>();
        for (var i = 0; i < count; i++)
        {
            var text = await cards.Nth(i).InnerTextAsync();
            names.Add(text);
        }

        return names;
    }

    public async Task SelectPlanAsync(string planName)
    {
        var card = Page.Locator(Selectors.Plans.PlanCard).Filter(new LocatorFilterOptions { HasText = planName });
        await card.Locator(Selectors.Plans.SelectButton).ClickAsync();
    }
}