// ReSharper disable InconsistentNaming
namespace dRofusClient.Projects;

/// <summary>
/// Dto for dRofus project.
/// </summary>
/// <schema>
/// {
///     "id": "string",
///     "constructor": "string",
///     "name": "string",
///     "planned_gross_area": 0,
///     "project_designed_gross_area": 0,
///     "project_gross_net_factor": 0,
///     "room_level_gross_net_factor": 0,
/// }
/// </schema>
public record dRofusProject : dRofusDto
{
    public string? Id { get; init; }
    public string? Constructor { get; init; }
    public string? Name { get; init; }
    public int? PlannedGrossArea { get; init; }
    public int? ProjectDesignedGrossArea { get; init; }
    public int? ProjectGrossNetFactor { get; init; }
    public int? RoomLevelGrossNetFactor { get; init; }
}