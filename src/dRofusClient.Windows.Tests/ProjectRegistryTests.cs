using dRofusClient.AttributeConfigurations;
using dRofusClient.Windows.Registries;
using Microsoft.Win32;

namespace dRofusClient.Windows.Tests;

public sealed class ProjectRegistryTests : IDisposable
{
    private readonly string _server = $"registry-tests-{Guid.NewGuid():N}";

    public static IEnumerable<object?[]> ConfigurationIds()
    {
        foreach (var configuration in new[] { AttributeConfigurationType.Article, AttributeConfigurationType.Occurence })
        {
            yield return [configuration, RegistryValueKind.DWord, 42, 42];
            yield return [configuration, RegistryValueKind.DWord, 0, null];
            yield return [configuration, RegistryValueKind.DWord, -1, null];
            yield return [configuration, RegistryValueKind.String, "42", 42];
            yield return [configuration, RegistryValueKind.String, "0", null];
            yield return [configuration, RegistryValueKind.String, "-1", null];
            yield return [configuration, RegistryValueKind.String, "invalid", null];
            yield return [configuration, RegistryValueKind.String, "", null];
            yield return [configuration, RegistryValueKind.String, "2147483648", null];
            yield return [configuration, RegistryValueKind.Binary, new byte[] { 42 }, null];
        }
    }

    [Theory]
    [MemberData(nameof(ConfigurationIds))]
    public void GetActiveAttributeConfigurationId_ReadsPositiveIds(
        AttributeConfigurationType configuration,
        RegistryValueKind valueKind,
        object value,
        int? expected)
    {
        using var key = Registry.CurrentUser.CreateSubKey(ProjectPath);
        key.SetValue(configuration.ToRegistryKey(), value, valueKind);

        var registry = new ProjectRegistry(_server, "test-database", "01");

        Assert.Equal(expected, registry.GetActiveAttributeConfigurationId(configuration));
    }

    [Theory]
    [InlineData(AttributeConfigurationType.Article)]
    [InlineData(AttributeConfigurationType.Occurence)]
    public void GetActiveAttributeConfigurationId_ReturnsNullWhenProjectIsMissing(AttributeConfigurationType configuration)
    {
        var registry = new ProjectRegistry(_server, "test-database", "01");

        Assert.Null(registry.GetActiveAttributeConfigurationId(configuration));
    }

    [Theory]
    [InlineData(AttributeConfigurationType.Article)]
    [InlineData(AttributeConfigurationType.Occurence)]
    public void GetActiveAttributeConfigurationId_ReturnsNullWhenSelectionIsMissing(AttributeConfigurationType configuration)
    {
        using var key = Registry.CurrentUser.CreateSubKey(ProjectPath);
        var registry = new ProjectRegistry(_server, "test-database", "01");

        Assert.Null(registry.GetActiveAttributeConfigurationId(configuration));
    }

    private string ProjectPath => RegistryExtensions.GetProjectRegistryPath(_server, "test-database", "01");

    public void Dispose()
    {
        Registry.CurrentUser.DeleteSubKeyTree(RegistryExtensions.GetServerRegistryPath(_server), throwOnMissingSubKey: false);
    }
}
