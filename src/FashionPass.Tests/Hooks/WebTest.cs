using FashionPass.Tests.Config;
using FashionPass.Tests.Drivers;
using FashionPass.Tests.Utilities;
using Microsoft.Playwright;
using NUnit.Framework.Interfaces;

namespace FashionPass.Tests.Hooks;

[Parallelizable(ParallelScope.Fixtures)]
public abstract class WebTest : BaseTest
{
    protected IPlaywright Playwright = null!;
    protected IBrowser Browser = null!;
    protected IBrowserContext Context = null!;
    protected IPage Page = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Config = TestConfig.Load();
        Playwright = await PlaywrightDriverFactory.CreatePlaywrightAsync();
        Browser = await PlaywrightDriverFactory.CreateBrowserAsync(Playwright, Config);
    }

    [SetUp]
    public async Task SetUp()
    {
        Context = await Browser.NewContextAsync(PlaywrightDriverFactory.BuildContextOptions(Config));
        Page = await Context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        var outcome = TestContext.CurrentContext.Result.Outcome.Status;
        if (Config.Screenshots.OnFailureOnly && outcome != TestStatus.Passed)
            await ScreenshotHelper.CaptureAsync(Page, Config, TestContext.CurrentContext.Test.Name);

        await Context.CloseAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Browser.CloseAsync();
        Playwright.Dispose();
    }
}