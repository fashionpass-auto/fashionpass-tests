using FashionPass.Tests.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace FashionPass.Tests.Pages.Mobile;

public sealed class HomeMobilePage : BaseMobilePage
{
    public HomeMobilePage(AppiumDriver driver, TestConfig config) : base(driver, config)
    {
    }

    public bool IsHomeScreenVisible()
    {
        var source = Driver.PageSource;
        return source.Contains("FashionPass", StringComparison.OrdinalIgnoreCase)
            || source.Contains("Rent", StringComparison.OrdinalIgnoreCase);
    }

    public void SearchFor(string term)
    {
        var searchField = By.XPath("//XCUIElementTypeSearchField | //android.widget.EditText");
        if (IsDisplayed(searchField))
        {
            EnterText(searchField, term);
            Find(searchField).SendKeys(Keys.Enter);
        }
    }
}