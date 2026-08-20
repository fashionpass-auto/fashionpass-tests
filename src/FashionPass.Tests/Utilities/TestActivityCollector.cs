using Microsoft.Playwright;

namespace FashionPass.Tests.Utilities;

public sealed class TestActivityCollector
{
    [ThreadStatic]
    private static TestActivityCollector? CurrentLocal;

    private readonly List<string> _consoleErrors = new();
    private readonly List<string> _failedResponses = new();
    private readonly List<string> _pageErrors = new();
    private readonly List<string> _actions = new();

    public static TestActivityCollector? Current
    {
        get => CurrentLocal;
        set => CurrentLocal = value;
    }

    public IReadOnlyList<string> ConsoleErrors => _consoleErrors;
    public IReadOnlyList<string> FailedResponses => _failedResponses;
    public IReadOnlyList<string> PageErrors => _pageErrors;
    public IReadOnlyList<string> Actions => _actions;

    public string? LastAction => _actions.Count > 0 ? _actions[^1] : null;

    public void RecordAction(string action) => _actions.Add(action);

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