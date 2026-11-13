using dRofusClient.AttributeConfigurations;
using System.Threading.Tasks;
using System;
using System.Linq;
using dRofusClient.Windows.Registries;

namespace dRofusClient.Windows.AttributeConfigurations;

public static class dRofusAttributeConfigurationExtensions
{
    public static Task<AttributeConfiguration> GetActiveAttributeConfigurationAsync(this IdRofusClient client, ProjectRegistry projectRegistry, AttributeConfigurationType attributeConfigurationType)
    {
        var occurenceConfigurationId = projectRegistry.GetActiveAttributeConfigurationId(attributeConfigurationType)
            ?? throw new InvalidOperationException("Occurence configuration id not set");
        return FetchAttributeConfigurationAsync(client, occurenceConfigurationId);
    }

    private static async Task<AttributeConfiguration> FetchAttributeConfigurationAsync(IdRofusClient client, int attributeConfigurationId)
    {
        var options = Query.List()
                    .Filter(Filter.Eq("id", attributeConfigurationId));

        var attributeConfigurations = await client.GetAttributeConfigurationsAsync(options).ConfigureAwait(false);

        if (attributeConfigurations.FirstOrDefault() is not { } attributeConfiguration)
            throw new InvalidOperationException($"Failed to get attribute configurations for id {attributeConfigurationId}");

        return attributeConfiguration;
    }

}