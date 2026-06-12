using ShopGuard.Core.Configuration;

namespace ShopGuard.E2ETests.Support;

/// <summary>
/// Process-wide access to the resolved <see cref="TestSettings"/>.
/// The target environment is selected via the SHOPGUARD_ENV variable (default: "local").
/// </summary>
public static class TestConfiguration
{
    private static readonly Lazy<TestSettings> LazySettings = new(() =>
    {
        var environment = Environment.GetEnvironmentVariable("SHOPGUARD_ENV") ?? "local";
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "appsettings.test.json");
        return SettingsLoader.Load(jsonPath, environment);
    });

    public static TestSettings Settings => LazySettings.Value;
}
