using Autodesk.Revit.DB;

namespace dRofusClient.Revit;

public static class dRofusRevitConnectionDetailsExtensions
{
    public static dRofusConnectionDetails ExtractdRofusConnectionDetails(this Document document)
    {
        var documentHash = document.GetHashCode();
        var projectInformation = document.ProjectInformation;

        var dataBase = GetParameter(projectInformation, "drofus_database")
            ?? throw new Exception("No dRofus database provided in model.");

        var server = GetParameter(projectInformation, "drofus_server")
            ?? throw new Exception("No dRofus server provided in model.");

        var projectId = GetParameter(projectInformation, "drofus_project_id")
            ?? throw new Exception("No dRofus project id provided in model.");

        var dRofusSyncSetupId = GetParameter(projectInformation, "drofus_sync_setup_id");

        int? syncSetupId = null;

        if (int.TryParse(dRofusSyncSetupId, out var id))
            syncSetupId = id;

        return new dRofusConnectionDetails
        {
            DocumentHash = documentHash,
            DataBase = dataBase,
            Server = server,
            ProjectId = projectId,
            DRofusSyncSetupId = syncSetupId
        };
    }

    private static string? GetParameter(ProjectInfo projectInformation, string parameterName)
    {
        using var parameter = projectInformation.LookupParameter(parameterName);

        if (parameter is null)
            return null;

        var stringValue = parameter.AsString();

        if (string.IsNullOrWhiteSpace(stringValue))
            return null;

        return stringValue;
    }
}