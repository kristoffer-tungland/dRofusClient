using Autodesk.Revit.DB;
using dRofusClient.Revit;
using dRofusClient.WindowsCredentials;

namespace dRofusClient.Revit;

public static class dRofusRevitClientExtensions
{
    public static IdRofusClient Create(
        this dRofusClientFactory dRofusClientFactory,
        dRofusConnectionDetails connectionInformation)
    {
        return dRofusClientFactory.Create(connectionInformation.Server, connectionInformation.DataBase, connectionInformation.ProjectId);
    }

    public static IdRofusClient Create(
        this dRofusClientFactory dRofusClientFactory,
        Document document)
    {
        var connectionInformation = document.ExtractdRofusConnectionDetails();
        return dRofusClientFactory.Create(connectionInformation.Server, connectionInformation.DataBase, connectionInformation.ProjectId);
    }
}
