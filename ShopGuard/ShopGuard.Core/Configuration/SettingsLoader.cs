using Microsoft.Extensions.Configuration;

namespace ShopGuard.Core.Configuration;

/// <summary>
/// Loads <see cref="TestSettings"/> from a JSON file and environment variables.
/// Precedence (highest wins): SHOPGUARD_* env vars > environment section > root section.
/// </summary>
public static class SettingsLoader
{
    public const string EnvVarPrefix = "SHOPGUARD_";

    /// <summary>
    /// Loads settings for the given environment (e.g. "local", "staging").
    /// The JSON file contains a root "TestSettings" section plus optional
    /// per-environment override sections ("Environments:{name}").
    /// </summary>
    public static TestSettings Load(string jsonPath, string environment = "local")
    {
        if (!File.Exists(jsonPath))
        {
            throw new FileNotFoundException($"Test configuration not found: {jsonPath}", jsonPath);
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(jsonPath, optional: false)
            .AddEnvironmentVariables(EnvVarPrefix)
            .Build();

        var settings = configuration.GetSection("TestSettings").Get<TestSettings>()
            ?? throw new InvalidOperationException("Section 'TestSettings' is missing or invalid.");

        // Apply per-environment overrides on top of the defaults.
        var overrides = configuration.GetSection($"Environments:{environment}");
        if (overrides.Exists())
        {
            settings = new TestSettings
            {
                ShopBaseUrl = overrides["ShopBaseUrl"] ?? settings.ShopBaseUrl,
                ApiBaseUrl = overrides["ApiBaseUrl"] ?? settings.ApiBaseUrl,
                ApiUsername = overrides["ApiUsername"] ?? settings.ApiUsername,
                ApiPassword = overrides["ApiPassword"] ?? settings.ApiPassword,
                DbConnectionString = overrides["DbConnectionString"] ?? settings.DbConnectionString,
                Headless = bool.TryParse(overrides["Headless"], out var headless) ? headless : settings.Headless,
                DefaultTimeoutMs = int.TryParse(overrides["DefaultTimeoutMs"], out var timeout)
                    ? timeout
                    : settings.DefaultTimeoutMs,
            };
        }

        // HEADLESS=false (CI convenience variable) wins over everything else.
        var headlessEnv = Environment.GetEnvironmentVariable("HEADLESS");
        if (bool.TryParse(headlessEnv, out var headlessOverride))
        {
            settings = settings with { Headless = headlessOverride };
        }

        return settings;
    }
}
