using System.Text.Json;
using System.Text.Json.Nodes;
using FashionPass.Tests.Data.Models;

namespace FashionPass.Tests.Config;

public sealed class TestConfig
{
    private const string EnvironmentVariable = "FASHIONPASS_ENV";

    public string EnvironmentName { get; set; } = "live";
    public SiteSettings Sites { get; set; } = new();
    public BrowserSettings Browser { get; set; } = new();
    public TimeoutSettings Timeouts { get; set; } = new();
    public ScreenshotSettings Screenshots { get; set; } = new();
    public MobileSettings Mobile { get; set; } = new();
    public UserSettings Users { get; set; } = new();
    public EmailSettings Email { get; set; } = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static TestConfig Load()
    {
        var basePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        var envName = System.Environment.GetEnvironmentVariable(EnvironmentVariable) ?? "production";

        var baseNode = JsonNode.Parse(File.ReadAllText(basePath));
        if (baseNode is not JsonObject merged)
            throw new InvalidOperationException($"Invalid configuration root in {basePath}");

        var envPath = Path.Combine(AppContext.BaseDirectory, $"appsettings.{envName}.json");
        if (File.Exists(envPath))
        {
            var envNode = JsonNode.Parse(File.ReadAllText(envPath));
            if (envNode is JsonObject envObject)
                merged = Merge(merged, envObject);
        }

        var config = merged.Deserialize<TestConfig>(JsonOptions) ?? new TestConfig();
        config.ApplyEnvironmentOverrides();
        return config;
    }

    private static JsonObject Merge(JsonObject target, JsonObject source)
    {
        foreach (var property in source)
        {
            if (target[property.Key] is JsonObject targetChild && property.Value is JsonObject sourceChild)
                target[property.Key] = Merge(targetChild, sourceChild);
            else
                target[property.Key] = property.Value?.DeepClone();
        }

        return target;
    }

    public string BaseUrl => Sites.Main.BaseUrl;

    private void ApplyEnvironmentOverrides()
    {
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_BASEURL") is { Length: > 0 } baseUrl)
            Sites.Main.BaseUrl = baseUrl;
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_BANJO_BASEURL") is { Length: > 0 } banjoUrl)
            Sites.Banjo.BaseUrl = banjoUrl;
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_LOKI_BASEURL") is { Length: > 0 } lokiUrl)
            Sites.Loki.BaseUrl = lokiUrl;
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_BROWSER_TYPE") is { Length: > 0 } browserType)
            Browser.Type = browserType;
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_BROWSER_CHANNEL") is { Length: > 0 } channel)
            Browser.Channel = channel;
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_HEADLESS") is { Length: > 0 } headless)
            Browser.Headless = bool.Parse(headless);
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_DEFAULT_TIMEOUT") is { Length: > 0 } timeout)
            Timeouts.Default = int.Parse(timeout);
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_NAV_TIMEOUT") is { Length: > 0 } navTimeout)
            Timeouts.Navigation = int.Parse(navTimeout);
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_MOBILE_ENABLED") is { Length: > 0 } mobileEnabled)
            Mobile.Enabled = bool.Parse(mobileEnabled);
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_EMAIL_ENABLED") is { Length: > 0 } emailEnabled)
            Email.Enabled = bool.Parse(emailEnabled);
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_SMTP_HOST") is { Length: > 0 } smtpHost)
            Email.SmtpHost = smtpHost;
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_SMTP_PORT") is { Length: > 0 } smtpPort)
            Email.SmtpPort = int.Parse(smtpPort);
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_SMTP_USER") is { Length: > 0 } smtpUser)
            Email.Username = smtpUser;
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_SMTP_PASS") is { Length: > 0 } smtpPass)
            Email.Password = smtpPass;
        if (System.Environment.GetEnvironmentVariable("FASHIONPASS_SMTP_FROM") is { Length: > 0 } smtpFrom)
            Email.From = smtpFrom;
    }
}

public sealed class BrowserSettings
{
    public string Type { get; set; } = "chromium";
    public string? Channel { get; set; }
    public bool Headless { get; set; }
    public int Width { get; set; } = 1440;
    public int Height { get; set; } = 900;
    public string Locale { get; set; } = "en-US";
    public string TimezoneId { get; set; } = "America/Los_Angeles";
    public bool IgnoreHttpsErrors { get; set; }
    public bool Trace { get; set; }
}

public sealed class TimeoutSettings
{
    public int Default { get; set; } = 30000;
    public int Navigation { get; set; } = 60000;
}

public sealed class ScreenshotSettings
{
    public string Directory { get; set; } = "test-results/screenshots";
    public bool OnFailureOnly { get; set; } = true;
}

public sealed class MobileSettings
{
    public bool Enabled { get; set; }
    public string PlatformName { get; set; } = "iOS";
    public string PlatformVersion { get; set; } = "17.2";
    public string DeviceName { get; set; } = "iPhone 15 Pro";
    public string AutomationName { get; set; } = "XCUITest";
    public string AppiumUrl { get; set; } = "http://127.0.0.1:4723";
    public string App { get; set; } = "";
    public string BundleId { get; set; } = "";
    public bool NoReset { get; set; } = true;
    public string Orientation { get; set; } = "PORTRAIT";
}

public sealed class UserSettings
{
    public User Default { get; set; } = new();
}

public sealed class SiteSettings
{
    public Site Main { get; set; } = new("https://www.fashionpass.com", "Main");
    public Site Banjo { get; set; } = new("https://banjo.fashionpass.com", "Banjo");
    public Site Loki { get; set; } = new("https://loki.fashionpass.com", "Loki");
}

public sealed class Site
{
    public Site() { }

    public Site(string baseUrl, string name)
    {
        BaseUrl = baseUrl;
        Name = name;
    }

    public string BaseUrl { get; set; } = "";
    public string Name { get; set; } = "";
}

public sealed class EmailSettings
{
    public bool Enabled { get; set; }
    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string From { get; set; } = "";
    public string[] To { get; set; } = Array.Empty<string>();
}