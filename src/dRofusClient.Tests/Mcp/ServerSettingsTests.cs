using dRofusClient.Mcp;
using System.Text;
using System.Text.Json;

namespace dRofusClient.Tests.Mcp;

public sealed class ServerSettingsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("false")]
    [InlineData("true")]
    public void EnvironmentPasswordWinsWithoutAccessingCredentialStore(string? enabled)
    {
        var values = ValidEnvironment();
        values["DROFUS_USE_WINDOWS_CREDENTIALS"] = enabled;
        var store = new FakeStore((_, _) => throw new Exception("Store must not be accessed"));
        var settings = ServerSettings.FromEnvironment(values.GetValueOrDefault, store);
        Assert.Equal(0, store.Calls);
        AssertPassword(settings, "test-password");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void OptInReadsSavedPasswordForConfiguredServerAndUsername(string? password)
    {
        var values = ValidEnvironment();
        values["DROFUS_PASSWORD"] = password;
        values["DROFUS_USE_WINDOWS_CREDENTIALS"] = "true";
        var store = new FakeStore((server, username) =>
        {
            Assert.Equal("https://api-no.drofus.com", server);
            Assert.Equal("test-user", username);
            return "saved-test-password";
        });
        var settings = ServerSettings.FromEnvironment(values.GetValueOrDefault, store);
        Assert.Equal(1, store.Calls);
        AssertPassword(settings, "saved-test-password");
        Assert.False(settings.EnableWrites);
        Assert.DoesNotContain("saved-test-password", JsonSerializer.Serialize(settings));
        Assert.DoesNotContain("test-user", JsonSerializer.Serialize(settings.Context));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("false")]
    public void MissingEnvironmentPasswordDoesNotImplicitlyReadStore(string? enabled)
    {
        var values = ValidEnvironment();
        values.Remove("DROFUS_PASSWORD");
        values["DROFUS_USE_WINDOWS_CREDENTIALS"] = enabled;
        var store = new FakeStore((_, _) => "saved-test-password");
        Assert.Throws<ArgumentException>(() => ServerSettings.FromEnvironment(values.GetValueOrDefault, store));
        Assert.Equal(0, store.Calls);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MissingOrEmptyStoredPasswordFailsClosed(string? password)
    {
        var values = ValidEnvironment();
        values.Remove("DROFUS_PASSWORD");
        values["DROFUS_USE_WINDOWS_CREDENTIALS"] = "true";
        var store = new FakeStore((_, _) => password);
        Assert.Throws<ArgumentException>(() => ServerSettings.FromEnvironment(values.GetValueOrDefault, store));
        Assert.Equal(1, store.Calls);
    }

    [Fact]
    public void StoreErrorsAreRedactedIncludingInnerException()
    {
        var values = ValidEnvironment();
        values.Remove("DROFUS_PASSWORD");
        values["DROFUS_USE_WINDOWS_CREDENTIALS"] = "true";
        var store = new FakeStore((_, _) => throw new InvalidOperationException("private-target saved-test-password"));
        var exception = Assert.Throws<ArgumentException>(() => ServerSettings.FromEnvironment(values.GetValueOrDefault, store));
        Assert.Null(exception.InnerException);
        Assert.DoesNotContain("private-target", exception.ToString());
        Assert.DoesNotContain("saved-test-password", exception.ToString());
    }

    [Fact]
    public void NonWindowsFallbackFailsWithoutLoadingNativeCredentialManager()
    {
        if (OperatingSystem.IsWindows())
            return;
        Assert.Throws<PlatformNotSupportedException>(() =>
            new WindowsCredentialPasswordStore().ReadPassword("https://api-no.drofus.com", "test-user"));
        var values = ValidEnvironment();
        values.Remove("DROFUS_PASSWORD");
        values["DROFUS_USE_WINDOWS_CREDENTIALS"] = "true";
        Assert.Throws<ArgumentException>(() => ServerSettings.FromEnvironment(values.GetValueOrDefault));
    }

    [Theory]
    [InlineData("https://api-no.drofus.com", "drofus://test-user@db2.nosyko.no")]
    [InlineData("https://api-no.drofus.com/", "drofus://test-user@db2.nosyko.no")]
    [InlineData("https://api-eu.drofus.com", "drofus://test-user@api-eu.drofus.com")]
    public void CredentialTargetsMatchExistingWindowsLibrary(string server, string target) =>
        Assert.Equal(target, WindowsCredentialPasswordStore.CreateTarget(server, "test-user"));

    private static void AssertPassword(ServerSettings settings, string expected)
    {
        var client = settings.CreateClient();
        using var httpClient = client.HttpClient;
        var authorization = httpClient.DefaultRequestHeaders.Authorization!;
        Assert.Equal("Basic", authorization.Scheme);
        Assert.Equal("test-user:" + expected, Encoding.UTF8.GetString(Convert.FromBase64String(authorization.Parameter!)));
    }

    private sealed class FakeStore(Func<string, string, string?> read) : IWindowsCredentialPasswordStore
    {
        public int Calls { get; private set; }
        public string? ReadPassword(string server, string username)
        {
            Calls++;
            return read(server, username);
        }
    }

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
    [InlineData("DROFUS_USE_WINDOWS_CREDENTIALS", "yes")]
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
