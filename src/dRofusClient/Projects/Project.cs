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
public record Project : dRofusIdDto
{
    [JsonPropertyName("constructor")]
    public string? Constructor { get; init; }
    public const string ConstructorField = "constructor";

    [JsonPropertyName("name")]
    public string? Name { get; init; }
    public const string NameField = "name";

    [JsonPropertyName("planned_gross_area")]
    public int? PlannedGrossArea { get; init; }
    public const string PlannedGrossAreaField = "planned_gross_area";

    [JsonPropertyName("project_designed_gross_area")]
    public int? ProjectDesignedGrossArea { get; init; }
    public const string ProjectDesignedGrossAreaField = "project_designed_gross_area";

    [JsonPropertyName("project_gross_net_factor")]
    public int? ProjectGrossNetFactor { get; init; }
    public const string ProjectGrossNetFactorField = "project_gross_net_factor";

    [JsonPropertyName("room_level_gross_net_factor")]
    public int? RoomLevelGrossNetFactor { get; init; }
    public const string RoomLevelGrossNetFactorField = "room_level_gross_net_factor";
}