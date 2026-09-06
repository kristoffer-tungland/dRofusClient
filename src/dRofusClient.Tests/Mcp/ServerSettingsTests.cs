using dRofusClient.Mcp;

namespace dRofusClient.Tests.Mcp;

public sealed class ServerSettingsTests
{
    [Fact]
    public void EnvironmentDefaultsToReadOnly()
    {
        var settings = ServerSettings.FromEnvironment(name => ValidEnvironment().GetValueOrDefault(name));
        Assert.False(settings.EnableWrites);
        Assert.Equal("test_db", settings.Context.Database);
    }

    [Theory]
    [InlineData("DROFUS_BASE_URL", "http://api-no.drofus.com")]
    [InlineData("DROFUS_BASE_URL", "******api-no.drofus.com")]
    [InlineData("DROFUS_BASE_URL", "https://api-no.drofus.com/other")]
    [InlineData("DROFUS_BASE_URL", "https://api-no.drofus.com/?query=1")]
    [InlineData("DROFUS_DATABASE", "../other")]
    [InlineData("DROFUS_PROJECT_ID", "1?other=2")]
    [InlineData("DROFUS_ENABLE_WRITES", "yes")]
    [InlineData("DROFUS_PASSWORD", "")]
    public void InvalidConfigurationFailsWithoutEchoingValues(string key, string value)
    {
        var values = ValidEnvironment();
        values[key] = value;
        var exception = Assert.Throws<ArgumentException>(() => ServerSettings.FromEnvironment(values.GetValueOrDefault));
        Assert.DoesNotContain("test-password", exception.Message);
    }

    private static Dictionary<string, string?> ValidEnvironment() => new()
    {
        ["DROFUS_BASE_URL"] = "https://api-no.drofus.com",
        ["DROFUS_DATABASE"] = "test_db",
        ["DROFUS_PROJECT_ID"] = "01",
        ["DROFUS_USERNAME"] = "test-user",
        ["DROFUS_PASSWORD"] = "test-password"
    };
}
