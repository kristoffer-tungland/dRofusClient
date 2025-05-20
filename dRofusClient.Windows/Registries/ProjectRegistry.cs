using dRofusClient.AttributeConfigurations;
using Microsoft.Win32;
using System;

namespace dRofusClient.Windows.Registries;

public static class RegistryKeyExtensions
{
    public static string ToRegistryKey(this AttributeConfigurationType registryKey)
    {
        return registryKey switch
        {
            AttributeConfigurationType.Occurence => "revitplugin-occurrence-configuration#",
            AttributeConfigurationType.Article => "revitplugin-article-configuration#",
            AttributeConfigurationType.ElectricalSystem => "revitplugin-electrical-system-configuration#",
            AttributeConfigurationType.MechanicalSystem => "revitplugin-mechanical-system-configuration#",
            AttributeConfigurationType.MechanicalSystemType => "revitplugin-mechanical-system-type-configuration#",
            AttributeConfigurationType.PipingSystem => "revitplugin-piping-system-configuration#",
            AttributeConfigurationType.PipingSystemType => "revitplugin-piping-system-type-configuration#",
            AttributeConfigurationType.ProjectInformationd => "revitplugin-project-information-configuration#",
            AttributeConfigurationType.Room => "revitplugin-room-configuration#",
            AttributeConfigurationType.RoomTemplate => "revitplugin-room-template-configuration#",
            AttributeConfigurationType.Space => "revitplugin-space-configuration#",
            AttributeConfigurationType.SpaceTemplate => "revitplugin-space-template-configuration#",
            _ => throw new ArgumentOutOfRangeException(nameof(registryKey), registryKey, null)
        };
    }
}

public class ProjectRegistry(string server, string database, string projectId)
{
    public int? GetActiveAttributeConfigurationId(AttributeConfigurationType registryKey)
    {
        var key = registryKey.ToRegistryKey();
        return GetConfigurationId(server, database, projectId, key);
    }

    private static int? GetConfigurationId(string server, string dataBase, string projectId, string registryKey)
    {
        var address = RegistryExtensions.GetProjectRegistryPath(server, dataBase, projectId);
        using var key = Registry.CurrentUser.OpenSubKey(address);
        var value = key?.GetValue(registryKey);
        if (int.TryParse(value as string, out var result))
        {
            if (result > 0)
                return result;

            return null;
        }

        return null;
    }
}