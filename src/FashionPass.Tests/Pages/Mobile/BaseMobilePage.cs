using FashionPass.Tests.Config;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace FashionPass.Tests.Pages.Mobile;

public abstract class BaseMobilePage
{
    protected readonly AppiumDriver Driver;
    protected readonly TestConfig Config;

    protected BaseMobilePage(AppiumDriver driver, TestConfig config)
    {
        Driver = driver;
        Config = config;
    }

    protected IWebElement Find(By by) => Driver.FindElement(by);

    protected bool IsDisplayed(By by) => Driver.FindElements(by).Any(element => element.Displayed);

    protected void Tap(By by) => Find(by).Click();

    protected void EnterText(By by, string text) => Find(by).SendKeys(text);

    protected string GetText(By by) => Find(by).Text;
}