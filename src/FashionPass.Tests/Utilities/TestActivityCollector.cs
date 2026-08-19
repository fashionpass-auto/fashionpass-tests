using Microsoft.Playwright;

namespace FashionPass.Tests.Utilities;

public sealed class TestActivityCollector
{
    private readonly List<string> _consoleErrors = new();
    private readonly List<string> _failedResponses = new();
    private readonly List<string> _pageErrors = new();

    public IReadOnlyList<string> ConsoleErrors => _consoleErrors;
    public IReadOnlyList<string> FailedResponses => _failedResponses;
    public IReadOnlyList<string> PageErrors => _pageErrors;

    public void Attach(IPage page)
    {
        page.Console += (_, e) =>
        {
            if (e.Type == "error")
                _consoleErrors.Add(e.Text);
        };

        page.Response += (_, e) =>
        {
            if (e.Status >= 400)
                _failedResponses.Add($"{e.Status} {e.Url}");
        };

        page.PageError += (_, e) => _pageErrors.Add(e);

        page.RequestFailed += (_, e) =>
            _pageErrors.Add($"Request failed: {e.Failure} {e.Url}");
    }
}