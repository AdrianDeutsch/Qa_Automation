namespace ShopGuard.Core.Configuration;

/// <summary>
/// Strongly typed test configuration, bound from appsettings.test.json
/// with environment-variable overrides (SHOPGUARD_ prefix).
/// </summary>
public sealed record TestSettings
{
    public required string ShopBaseUrl { get; init; }

    public required string ApiBaseUrl { get; init; }

    public required string ApiUsername { get; init; }

    public required string ApiPassword { get; init; }

    public required string DbConnectionString { get; init; }

    public bool Headless { get; init; } = true;

    public int DefaultTimeoutMs { get; init; } = 30_000;
}
