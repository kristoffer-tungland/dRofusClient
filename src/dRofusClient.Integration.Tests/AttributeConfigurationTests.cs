using dRofusClient.AttributeConfigurations;

namespace dRofusClient.Integration.Tests;

public class AttributeConfigurationTests(SetupFixture fixture) : IClassFixture<SetupFixture>
{
    private readonly IdRofusClient _client = fixture.Client;

    [Fact]
    public async Task GetAttributeConfigurations_ShouldReturnValidConfigTypes()
    {
        var attributeConfigurations = await _client.GetAttributeConfigurationsAsync();

        Assert.NotEmpty(attributeConfigurations);

        var configTypes = attributeConfigurations
            .Select(x => x.ConfigType)
            .Distinct()
            .ToList();

        Assert.NotEmpty(configTypes);

        var nonNullConfigTypes = configTypes
            .OfType<string>()
            .ToList();

        Assert.NotEmpty(nonNullConfigTypes);
        Assert.Equal(configTypes.Count, nonNullConfigTypes.Count);

        foreach (var configType in nonNullConfigTypes)
        {
            var attributeConfigType = AttributeConfigTypeExtensions.FromRequest(configType);
            Assert.NotEqual(AttributeConfigType.Unknown, attributeConfigType);
        }
    }

    [Fact]
    public async Task GetAttributeConfigurations_WithType_ShouldReturnFilteredConfigs()
    {
        var attributeConfigurations = await _client.GetAttributeConfigurationsAsync(AttributeConfigType.RevitOccurrence);
        Assert.NotEmpty(attributeConfigurations);

        foreach (var config in attributeConfigurations)
        {
            Assert.Equal(AttributeConfigType.RevitOccurrence.ToRequest(), config.ConfigType);
        }
    }

    [Fact]
    public async Task GetAttributeConfiguration_ShouldReturnSingleConfig()
    {
        var attributeConfigurations = await _client.GetAttributeConfigurationsAsync();
        Assert.NotEmpty(attributeConfigurations);
        var firstConfig = attributeConfigurations.First();

        Assert.NotNull(firstConfig);
        Assert.NotEqual(0, firstConfig.Id);

        var fetchedConfig = await _client.GetAttributeConfigurationAsync(firstConfig.Id!.Value);
        Assert.NotNull(fetchedConfig);
        Assert.Equal(firstConfig.Id, fetchedConfig?.Id);
        Assert.Equal(firstConfig.Name, fetchedConfig?.Name);
        Assert.Equal(firstConfig.ConfigType, fetchedConfig?.ConfigType);
    }

    [Fact]
    public async Task GetAttributeConfigurations_WithAvailableToUsers_ShouldReturnFilteredConfigs()
    {
        var attributeConfigurations = await _client.GetAttributeConfigurationsAsync(AttributeConfigType.Room, true);
        Assert.NotEmpty(attributeConfigurations);

        foreach (var config in attributeConfigurations)
        {
            Assert.Equal(AttributeConfigType.Room, config.ConfigTypeEnum);
            Assert.True(config.AvailableToUsers);
        }
    }

    [Fact]
    public async Task GetAttributeConfigurations_WithUnavailableToUsers_ShouldReturnFilteredConfigs()
    {
        var attributeConfigurations = await _client.GetAttributeConfigurationsAsync(AttributeConfigType.Room, false);
        Assert.NotEmpty(attributeConfigurations);

        foreach (var config in attributeConfigurations)
        {
            Assert.Equal(AttributeConfigType.Room, config.ConfigTypeEnum);
            Assert.False(config.AvailableToUsers);
        }
    }

    [Fact]
    public async Task GetAttributeConfigurations_WithDefaultRoomConfig_ShouldReturnSingleDefaultRoomConfig()
    {
        var query = Query.List()
            .Filters(
                Filter.Eq(AttributeConfiguration.ConfigTypeField, AttributeConfigType.Room),
                Filter.Eq(AttributeConfiguration.IsDefaultField, true));

        var attributeConfigurations = await _client.GetAttributeConfigurationsAsync(query);

        Assert.NotEmpty(attributeConfigurations);
        Assert.Single(attributeConfigurations);

        var config = attributeConfigurations.First();

        Assert.Equal(AttributeConfigType.Room, config.ConfigTypeEnum);
        Assert.True(config.IsDefault);
    }
}