using FashionPass.Tests.Config;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

namespace FashionPass.Tests.Drivers;

public static class MobileDriverFactory
{
    public static AppiumDriver CreateDriver(TestConfig config)
    {
        var options = new AppiumOptions
        {
            PlatformName = config.Mobile.PlatformName,
            PlatformVersion = config.Mobile.PlatformVersion,
            DeviceName = config.Mobile.DeviceName,
            AutomationName = config.Mobile.AutomationName
        };

        if (!string.IsNullOrEmpty(config.Mobile.App))
            options.App = config.Mobile.App;

        if (!string.IsNullOrEmpty(config.Mobile.BundleId))
            options.AddAdditionalAppiumOption("bundleId", config.Mobile.BundleId);

        if (config.Mobile.NoReset)
            options.AddAdditionalAppiumOption("noReset", true);

        options.AddAdditionalAppiumOption("orientation", config.Mobile.Orientation);

        var serverUri = new Uri(config.Mobile.AppiumUrl);
        var isIos = config.Mobile.PlatformName.Equals("iOS", StringComparison.OrdinalIgnoreCase);

        return isIos
            ? new IOSDriver(serverUri, options)
            : new AndroidDriver(serverUri, options);
    }
}