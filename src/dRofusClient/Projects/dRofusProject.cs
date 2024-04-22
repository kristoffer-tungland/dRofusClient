// ReSharper disable InconsistentNaming

namespace dRofusClient.Projects;

/// <summary>
/// Dto for dRofus project.
/// </summary>
/// <param name="Id">The id of the project.</param>
/// <param name="Constructor"></param>
/// <param name="Name"></param>
/// <param name="PlannedGrossArea"></param>
/// <param name="ProjectDesignedGrossArea"></param>
/// <param name="ProjectGrossNetFactor"></param>
/// <param name="RoomLevelGrossNetFactor"></param>
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
public record dRofusProject(
    [property: JsonProperty("id")] string Id,
    [property: JsonProperty("constructor")] string Constructor,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("planned_gross_area")] int? PlannedGrossArea,
    [property: JsonProperty("project_designed_gross_area")] int? ProjectDesignedGrossArea,
    [property: JsonProperty("project_gross_net_factor")] int? ProjectGrossNetFactor,
    [property: JsonProperty("room_level_gross_net_factor")] int? RoomLevelGrossNetFactor
) : dRofusDto;