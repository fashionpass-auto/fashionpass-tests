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
    protected TestActivityCollector Activity = null!;
    private bool _tracingStarted;

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
        Activity = new TestActivityCollector();
        Activity.Attach(Page);
        TestActivityCollector.Current = Activity;

        if (Config.Browser.Trace || Config.Email.Enabled)
        {
            await TraceHelper.StartAsync(Context);
            _tracingStarted = true;
        }
    }

    [TearDown]
    public async Task TearDown()
    {
        var outcome = TestContext.CurrentContext.Result.Outcome.Status;
        var testName = TestContext.CurrentContext.Test.Name;

        string? screenshotPath = null;
        string? tracePath = null;

        if (outcome != TestStatus.Passed)
        {
            screenshotPath = await ScreenshotHelper.CaptureAsync(Page, Config, testName);
            if (_tracingStarted)
                tracePath = await TraceHelper.StopAndSaveAsync(Context, Config, testName);

            if (Config.Email.Enabled)
                await EmailReporter.SendFailureReportAsync(
                    Config,
                    testName,
                    Activity,
                    Page,
                    screenshotPath,
                    tracePath);
        }
        else if (_tracingStarted)
        {
            await TraceHelper.StopDiscardAsync(Context);
        }

        await Context.CloseAsync();
        TestActivityCollector.Current = null;
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Browser.CloseAsync();
        Playwright.Dispose();
    }
}