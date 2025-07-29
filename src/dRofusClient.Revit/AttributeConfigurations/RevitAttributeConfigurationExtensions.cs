using dRofusClient.Options;
using dRofusClient.Revit.Utils;
using dRofusClient.Windows.Registries;
using dRofusClient.Windows.AttributeConfigurations;
using dRofusClient.Revit;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace dRofusClient.AttributeConfigurations;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class RevitAttributeConfigurationExtensions
{
    public static ProjectRegistry ToProjectRegistry(this dRofusConnectionDetails connectionDetails)
    {
        return new ProjectRegistry(connectionDetails.Server, connectionDetails.DataBase, connectionDetails.ProjectId);
    }

    public static AttributeConfiguration GetActiveAttributeConfiguration(this IdRofusClient client, ProjectRegistry projectRegistry, AttributeConfigurationType attributeConfigurationType)
    {
        return AsyncUtil.RunSync(() => client.GetActiveAttributeConfigurationAsync(projectRegistry, attributeConfigurationType));
    }

    public static List<AttributeConfiguration> GetAttributeConfigurations(this IdRofusClient client, ListQuery options, CancellationToken cancellationToken = default)
    {
        return AsyncUtil.RunSync(() => client.GetAttributeConfigurationsAsync(options, cancellationToken));
    }

    public static AttributeConfiguration? GetAttributeConfiguration(this IdRofusClient client, int attributeConfigurationId, CancellationToken cancellationToken = default)
    {
        return AsyncUtil.RunSync(() => client.GetAttributeConfigurationAsync(attributeConfigurationId, cancellationToken));
    }
}
