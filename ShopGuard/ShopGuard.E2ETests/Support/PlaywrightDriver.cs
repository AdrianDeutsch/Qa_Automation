using Microsoft.Playwright;

namespace ShopGuard.E2ETests.Support;

/// <summary>
/// Scenario-scoped Playwright driver (injected by Reqnroll context injection).
/// The browser process is shared across scenarios of a worker for speed;
/// every scenario gets an isolated BrowserContext with tracing enabled.
/// </summary>
public sealed class PlaywrightDriver : IAsyncDisposable
{
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private static readonly SemaphoreSlim BrowserLock = new(1, 1);

    private IBrowserContext? _context;
    private IPage? _page;

    /// <summary>The page of the current scenario. Only valid after <see cref="InitAsync"/>.</summary>
    public IPage Page => _page ?? throw new InvalidOperationException(
        "PlaywrightDriver is not initialized. Did the scenario miss the @ui tag?");

    /// <summary>Creates the scenario's browser context and starts tracing.</summary>
    public async Task InitAsync()
    {
        await BrowserLock.WaitAsync();
        try
        {
            _playwright ??= await Playwright.CreateAsync();
            _browser ??= await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = TestConfiguration.Settings.Headless,
            });
        }
        finally
        {
            BrowserLock.Release();
        }

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1600, Height = 900 },
        });
        _context.SetDefaultTimeout(TestConfiguration.Settings.DefaultTimeoutMs);

        // Tracing captures screenshots, snapshots and sources for the trace viewer.
        await _context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true,
        });

        _page = await _context.NewPageAsync();
    }

    /// <summary>Saves the trace of the current context (call only for failed scenarios).</summary>
    public async Task SaveTraceAsync(string tracePath)
    {
        if (_context is not null)
        {
            await _context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
        }
    }

    /// <summary>Discards the trace without writing a file (for passed scenarios).</summary>
    public async Task DiscardTraceAsync()
    {
        if (_context is not null)
        {
            await _context.Tracing.StopAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_page is not null)
        {
            await _page.CloseAsync();
        }

        if (_context is not null)
        {
            await _context.CloseAsync();
        }
    }
}
