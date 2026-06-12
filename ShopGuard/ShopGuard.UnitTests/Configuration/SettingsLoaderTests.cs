using AwesomeAssertions;
using ShopGuard.Core.Configuration;

namespace ShopGuard.UnitTests.Configuration;

[TestFixture]
[NonParallelizable] // Tests mutate process-wide environment variables.
public sealed class SettingsLoaderTests
{
    private string _jsonPath = null!;

    [SetUp]
    public void WriteSettingsFile()
    {
        _jsonPath = Path.Combine(Path.GetTempPath(), $"shopguard-settings-{Guid.NewGuid():N}.json");
        File.WriteAllText(_jsonPath, """
            {
              "TestSettings": {
                "ShopBaseUrl": "https://demo.nopcommerce.com",
                "ApiBaseUrl": "https://restful-booker.herokuapp.com",
                "ApiUsername": "admin",
                "ApiPassword": "password123",
                "DbConnectionString": "Data Source=shopguard.db",
                "Headless": true,
                "DefaultTimeoutMs": 30000
              },
              "Environments": {
                "staging": {
                  "ShopBaseUrl": "https://staging.example.com",
                  "Headless": "false"
                }
              }
            }
            """);
        Environment.SetEnvironmentVariable("HEADLESS", null);
    }

    [TearDown]
    public void Cleanup()
    {
        File.Delete(_jsonPath);
        Environment.SetEnvironmentVariable("HEADLESS", null);
    }

    [Test]
    public void Load_ReadsRootSection()
    {
        var settings = SettingsLoader.Load(_jsonPath);

        settings.ShopBaseUrl.Should().Be("https://demo.nopcommerce.com");
        settings.ApiUsername.Should().Be("admin");
        settings.Headless.Should().BeTrue();
        settings.DefaultTimeoutMs.Should().Be(30000);
    }

    [Test]
    public void Load_AppliesEnvironmentSectionOverrides()
    {
        var settings = SettingsLoader.Load(_jsonPath, environment: "staging");

        settings.ShopBaseUrl.Should().Be("https://staging.example.com");
        settings.Headless.Should().BeFalse();
        // Values not overridden fall back to the root section.
        settings.ApiBaseUrl.Should().Be("https://restful-booker.herokuapp.com");
    }

    [Test]
    public void Load_UnknownEnvironment_FallsBackToRootSection()
    {
        var settings = SettingsLoader.Load(_jsonPath, environment: "does-not-exist");

        settings.ShopBaseUrl.Should().Be("https://demo.nopcommerce.com");
    }

    [Test]
    public void Load_HeadlessEnvVariable_WinsOverFile()
    {
        Environment.SetEnvironmentVariable("HEADLESS", "false");

        var settings = SettingsLoader.Load(_jsonPath);

        settings.Headless.Should().BeFalse();
    }

    [Test]
    public void Load_MissingFile_ThrowsFileNotFound()
    {
        var act = () => SettingsLoader.Load("/nonexistent/appsettings.test.json");

        act.Should().Throw<FileNotFoundException>();
    }
}
