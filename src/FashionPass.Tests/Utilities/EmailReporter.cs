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

        var body = BuildBody(pageTitle, activity, screenshotPath, tracePath);

#pragma warning disable CS0618
        using var message = new MailMessage
        {
            From = new MailAddress(
                string.IsNullOrWhiteSpace(config.Email.From) ? config.Email.Username : config.Email.From),
            Subject = $"Test Failed: {testName}",
            IsBodyHtml = false,
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

    private static string BuildBody(
        string pageTitle,
        TestActivityCollector? activity,
        string? screenshotPath,
        string? tracePath)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"page title: {pageTitle}");

        var lastAction = activity?.LastAction;
        sb.AppendLine(string.IsNullOrWhiteSpace(lastAction) ? "failed on unknown" : $"failed on {lastAction}");

        foreach (var api in activity?.FailedResponses ?? new List<string>())
            sb.AppendLine($"failed on API: {api}");

        if (!string.IsNullOrWhiteSpace(screenshotPath) && File.Exists(screenshotPath))
            sb.AppendLine($"At the time of failure Screenshot: {Path.GetFileName(screenshotPath)}");

        if (!string.IsNullOrWhiteSpace(tracePath) && File.Exists(tracePath))
            sb.AppendLine($"trace report: {Path.GetFileName(tracePath)}");

        sb.AppendLine("----------");
        sb.AppendLine("end");

        return sb.ToString();
    }

    private static bool IsTransient(SmtpStatusCode statusCode) => statusCode is
        SmtpStatusCode.GeneralFailure or
        SmtpStatusCode.ServiceNotAvailable or
        SmtpStatusCode.MailboxBusy or
        SmtpStatusCode.LocalErrorInProcessing or
        SmtpStatusCode.InsufficientStorage;
}