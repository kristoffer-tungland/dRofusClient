namespace dRofusClient.Revit;

public record dRofusConnectionDetails
{
    public required int DocumentHash { get; init; }
    public required string DataBase { get; init; }
    public required string Server { get; init;}
    public required string ProjectId { get; init; }
    public required int? DRofusSyncSetupId { get; init; }
}