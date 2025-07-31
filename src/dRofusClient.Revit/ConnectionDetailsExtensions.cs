using dRofusClient.Windows.Registries;

namespace dRofusClient.Revit;

public static class ConnectionDetailsExtensions
{
    public static ProjectRegistry ToProjectRegistry(this dRofusConnectionDetails connectionDetails)
    {
        return new ProjectRegistry(connectionDetails.Server, connectionDetails.DataBase, connectionDetails.ProjectId);
    }
}
