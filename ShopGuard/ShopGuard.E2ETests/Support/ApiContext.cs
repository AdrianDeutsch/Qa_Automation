using ShopGuard.Core.Api;

namespace ShopGuard.E2ETests.Support;

/// <summary>
/// Scenario-scoped holder for the typed API client. The HttpClient lives for the
/// whole scenario so the auth token set on the client survives between steps.
/// </summary>
public sealed class ApiContext : IDisposable
{
    private readonly Lazy<(HttpClient Http, BookingApiClient Client)> _lazy = new(() =>
    {
        var http = new HttpClient
        {
            // Trailing slash is required so relative paths like "booking/1" resolve correctly.
            BaseAddress = new Uri(TestConfiguration.Settings.ApiBaseUrl.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(60),
        };
        return (http, new BookingApiClient(http));
    });

    public BookingApiClient Client => _lazy.Value.Client;

    public void Dispose()
    {
        if (_lazy.IsValueCreated)
        {
            _lazy.Value.Http.Dispose();
        }
    }
}
