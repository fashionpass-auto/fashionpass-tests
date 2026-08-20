using System.Net;
using System.Net.Mail;
using System.Text;
using FashionPass.Tests.Config;
using Microsoft.Playwright;

namespace FashionPass.Tests.Utilities;

public static class EmailReporter
{
    public static async Task SendFailureReportAsync(
        TestConfig config,
        string testName,
        string? failureMessage,
        TestActivityCollector? activity,
        IPage page,
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

        var pageTitle = string.Empty;
        if (page is not null)
        {
            try { pageTitle = await page.TitleAsync(); }
            catch (PlaywrightException) { pageTitle = string.Empty; }
        }

        var body = BuildHtmlBody(config, testName, pageTitle, failureMessage, activity, screenshotPath, tracePath);

#pragma warning disable CS0618
        using var message = new MailMessage
        {
            From = new MailAddress(
                string.IsNullOrWhiteSpace(config.Email.From) ? config.Email.Username : config.Email.From),
            Subject = $"[FashionPass Tests] Test Failed: {testName}",
            IsBodyHtml = true,
            Body = body
        };

        foreach (var recipient in config.Email.To)
            message.To.Add(recipient);

        if (!string.IsNullOrWhiteSpace(screenshotPath) && File.Exists(screenshotPath))
            message.Attachments.Add(new Attachment(screenshotPath));

        if (!string.IsNullOrWhiteSpace(tracePath) && File.Exists(tracePath))
            message.Attachments.Add(new Attachment(tracePath));

        const int maxAttempts = 3;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var client = new SmtpClient(config.Email.SmtpHost, config.Email.SmtpPort)
                {
                    EnableSsl = config.Email.UseSsl,
                    Credentials = new NetworkCredential(config.Email.Username, config.Email.Password),
                    Timeout = 60000
                };

                await client.SendMailAsync(message);
                Console.WriteLine($"[EmailReporter] Failure report sent to {string.Join(", ", config.Email.To)}");
                return;
            }
            catch (SmtpException ex) when (IsTransient(ex.StatusCode) && attempt < maxAttempts)
            {
                Console.WriteLine($"[EmailReporter] Transient SMTP error (attempt {attempt}/{maxAttempts}): {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(attempt * 5));
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"[EmailReporter] FAILED to send email to {string.Join(", ", config.Email.To)}: {ex.Message}");
                return;
            }
        }
#pragma warning restore CS0618
    }

    private static bool IsTransient(SmtpStatusCode statusCode) => statusCode is
        SmtpStatusCode.GeneralFailure or
        SmtpStatusCode.ServiceNotAvailable or
        SmtpStatusCode.MailboxBusy or
        SmtpStatusCode.LocalErrorInProcessing or
        SmtpStatusCode.InsufficientStorage;

    private static string BuildHtmlBody(
        TestConfig config,
        string testName,
        string pageTitle,
        string? failureMessage,
        TestActivityCollector? activity,
        string? screenshotPath,
        string? tracePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html><html><body style='font-family: Arial, sans-serif;'>");
        sb.AppendLine("<h2 style='color:#d93025;'>Test Failed: " + HtmlEncode(testName) + "</h2>");
        sb.AppendLine("<table cellpadding='4' style='border-collapse:collapse;'>");
        sb.AppendLine(Row("Environment", config.EnvironmentName));
        sb.AppendLine(Row("Page Title", pageTitle));
        sb.AppendLine(Row("Last Action", activity?.LastAction ?? "None recorded"));
        sb.AppendLine(Row("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        sb.AppendLine("</table>");

        sb.AppendLine("<h3 style='color:#333;'>Failure</h3>");
        sb.AppendLine("<pre style='background:#f6f8fa;padding:10px;border-radius:4px;'>" +
                      HtmlEncode(failureMessage ?? "No failure message captured") + "</pre>");

        var failedApis = activity?.FailedResponses ?? new List<string>();
        if (failedApis.Count > 0)
        {
            sb.AppendLine("<h3 style='color:#333;'>Failed API requests</h3>");
            sb.AppendLine("<ul>");
            foreach (var api in failedApis.Take(50))
                sb.AppendLine("<li>" + HtmlEncode(api) + "</li>");
            sb.AppendLine("</ul>");
        }

        if (screenshotPath is not null && File.Exists(screenshotPath))
            sb.AppendLine("<p><b>Screenshot:</b> attached (" + HtmlEncode(Path.GetFileName(screenshotPath)) + ")</p>");

        if (tracePath is not null && File.Exists(tracePath))
        {
            sb.AppendLine("<p><b>Trace report:</b> attached (" + HtmlEncode(Path.GetFileName(tracePath)) + ")</p>");
            sb.AppendLine("<p>Open the trace with: <code>npx playwright show-trace " +
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