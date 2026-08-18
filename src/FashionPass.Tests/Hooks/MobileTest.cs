using FashionPass.Tests.Config;
using FashionPass.Tests.Drivers;
using OpenQA.Selenium.Appium;

namespace FashionPass.Tests.Hooks;

public abstract class MobileTest : BaseTest
{
    protected AppiumDriver Driver = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Config = TestConfig.Load();
        if (!Config.Mobile.Enabled)
            Assert.Ignore("Mobile tests are disabled. Set Mobile.Enabled=true and start an Appium server first.");

        Driver = MobileDriverFactory.CreateDriver(Config);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Driver?.Quit();
    }
}