using System.Net;
using System.Net.Mail;
using System.Text;
using FashionPass.Tests.Config;

namespace FashionPass.Tests.Utilities;

public static class EmailReporter
{
    public static async Task SendFailureReportAsync(
        TestConfig config,
        string testName,
        string? failureMessage,
        TestActivityCollector? activity,
        string? screenshotPath,
        string? tracePath)
    {
        if (!config.Email.Enabled)
            return;

        if (string.IsNullOrWhiteSpace(config.Email.SmtpHost)
            || string.IsNullOrWhiteSpace(config.Email.Username)
            || string.IsNullOrWhiteSpace(config.Email.Password))
        {
            Console.WriteLine($"[EmailReporter] Skipped: SMTP credentials not configured. " +
                              $"Set FASHIONPASS_SMTP_USER / FASHIONPASS_SMTP_PASS to enable email reports.");
            return;
        }

        var body = BuildHtmlBody(config, testName, failureMessage, activity, screenshotPath, tracePath);

#pragma warning disable CS0618
        using var message = new MailMessage
        {
            From = new MailAddress(
                string.IsNullOrWhiteSpace(config.Email.From) ? config.Email.Username : config.Email.From),
            Subject = $"[FashionPass Tests] FAILED: {testName} ({config.EnvironmentName})",
            IsBodyHtml = true,
            Body = body
        };

        foreach (var recipient in config.Email.To)
            message.To.Add(recipient);

        if (!string.IsNullOrWhiteSpace(screenshotPath) && File.Exists(screenshotPath))
            message.Attachments.Add(new Attachment(screenshotPath));

        if (!string.IsNullOrWhiteSpace(tracePath) && File.Exists(tracePath))
            message.Attachments.Add(new Attachment(tracePath));

        using var client = new SmtpClient(config.Email.SmtpHost, config.Email.SmtpPort)
        {
            EnableSsl = config.Email.UseSsl,
            Credentials = new NetworkCredential(config.Email.Username, config.Email.Password),
            Timeout = 60000
        };

        await client.SendMailAsync(message);
        Console.WriteLine($"[EmailReporter] Failure report sent to {string.Join(", ", config.Email.To)}");
#pragma warning restore CS0618
    }

    private static string BuildHtmlBody(
        TestConfig config,
        string testName,
        string? failureMessage,
        TestActivityCollector? activity,
        string? screenshotPath,
        string? tracePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html><html><body style='font-family: Arial, sans-serif;'>");
        sb.AppendLine("<h2 style='color:#d93025;'>Test Failed: " + HtmlEncode(testName) + "</h2>");
        sb.AppendLine("<table cellpadding='6' style='border-collapse:collapse;'>");
        sb.AppendLine(Row("Environment", config.EnvironmentName));
        sb.AppendLine(Row("Site", config.BaseUrl));
        sb.AppendLine(Row("Browser", config.Browser.Type + (string.IsNullOrWhiteSpace(config.Browser.Channel) ? "" : " (" + config.Browser.Channel + ")")));
        sb.AppendLine(Row("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        sb.AppendLine("</table>");

        sb.AppendLine("<h3 style='color:#333;'>Event that failed</h3>");
        sb.AppendLine("<pre style='background:#f6f8fa;padding:10px;border-radius:4px;'>" +
                      HtmlEncode(failureMessage ?? "No failure message captured") + "</pre>");

        sb.AppendLine("<h3 style='color:#333;'>Failed API requests</h3>");
        var failedApis = activity?.FailedResponses ?? new List<string>();
        if (failedApis.Count > 0)
        {
            sb.AppendLine("<ul>");
            foreach (var api in failedApis.Take(50))
                sb.AppendLine("<li>" + HtmlEncode(api) + "</li>");
            sb.AppendLine("</ul>");
        }
        else
        {
            sb.AppendLine("<p>No failed HTTP requests (4xx/5xx) were recorded.</p>");
        }

        var consoleErrors = activity?.ConsoleErrors ?? new List<string>();
        if (consoleErrors.Count > 0)
        {
            sb.AppendLine("<h3 style='color:#333;'>Console errors</h3>");
            sb.AppendLine("<ul>");
            foreach (var error in consoleErrors.Take(20))
                sb.AppendLine("<li>" + HtmlEncode(error) + "</li>");
            sb.AppendLine("</ul>");
        }

        if (screenshotPath is not null && File.Exists(screenshotPath))
        {
            sb.AppendLine("<h3 style='color:#333;'>Screenshot</h3>");
            sb.AppendLine("<p>Attached: <code>" + HtmlEncode(Path.GetFileName(screenshotPath)) + "</code></p>");
        }

        if (tracePath is not null && File.Exists(tracePath))
        {
            sb.AppendLine("<h3 style='color:#333;'>Playwright Trace</h3>");
            sb.AppendLine("<p>Attached: <code>" + HtmlEncode(Path.GetFileName(tracePath)) + "</code></p>");
            sb.AppendLine("<p>Open it locally with: <code>npx playwright show-trace " +
                          HtmlEncode(Path.GetFileName(tracePath)) + "</code></p>");
        }

        sb.AppendLine("</body></html>");
        return sb.ToString();
    }

    private static string Row(string label, string value)
        => "<tr><td style='padding-right:16px;font-weight:bold;color:#555;'>" + HtmlEncode(label) +
           "</td><td>" + HtmlEncode(value) + "</td></tr>";

    private static string HtmlEncode(string value) => WebUtility.HtmlEncode(value);
}