using dRofusClient.Options;
using dRofusClient.Revit.Utils;
using dRofusClient.AttributeConfigurations;
using dRofusClient.Windows.Registries;
using dRofusClient.Windows.AttributeConfigurations;

namespace dRofusClient.Revit.AttributeConfigurations;

public static class RevitAttributeConfigurationExtensions
{
    public static ProjectRegistry ToProjectRegistry(this dRofusConnectionDetails connectionDetails)
    {
        return new ProjectRegistry(connectionDetails.Server, connectionDetails.DataBase, connectionDetails.ProjectId);
    }

    public static dRofusAttributeConfiguration GetActiveAttributeConfiguration(this IdRofusClient client, ProjectRegistry projectRegistry, AttributeConfigurationType attributeConfigurationType)
    {
        return AsyncUtil.RunSync(() => client.GetActiveAttributeConfigurationAsync(projectRegistry, attributeConfigurationType));
    }

    public static List<dRofusAttributeConfiguration> GetAttributeConfigurations(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return AsyncUtil.RunSync(() => client.GetAttributeConfigurationsAsync(options, cancellationToken));
    }
}
