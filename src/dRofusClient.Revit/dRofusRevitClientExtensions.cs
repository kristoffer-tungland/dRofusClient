using Autodesk.Revit.DB;
using dRofusClient.Revit;
using dRofusClient.Windows;

namespace dRofusClient;

public static class dRofusRevitClientExtensions
{
    public static IdRofusClient Create(
        this dRofusClientFactory dRofusClientFactory,
        dRofusConnectionDetails connectionInformation,
        ILoginPromptHandler? loginPromtHandler = default)
    {
        return dRofusClientFactory.Create(connectionInformation.Server, connectionInformation.DataBase, connectionInformation.ProjectId);
    }

    public static IdRofusClient Create(
        this dRofusClientFactory dRofusClientFactory,
        Document document,
        ILoginPromptHandler? loginPromtHandler = default)
    {
        var connectionInformation = document.ExtractdRofusConnectionDetails();
        return dRofusClientFactory.Create(connectionInformation.Server, connectionInformation.DataBase, connectionInformation.ProjectId);
    }
}
