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
    [JsonPropertyName("constructor")]
    public string? Constructor { get; init; }
    /// <summary>Field name for Constructor, used in filters and order by clauses.</summary>
    /// <returns>"constructor"</returns>
    public static string ConstructorField => "constructor";

    [JsonPropertyName("name")]
    public string? Name { get; init; }
    /// <summary>Field name for Name, used in filters and order by clauses.</summary>
    /// <returns>"name"</returns>
    public static string NameField => "name";

    [JsonPropertyName("planned_gross_area")]
    public int? PlannedGrossArea { get; init; }
    /// <summary>Field name for PlannedGrossArea, used in filters and order by clauses.</summary>
    /// <returns>"planned_gross_area"</returns>
    public static string PlannedGrossAreaField => "planned_gross_area";

    [JsonPropertyName("project_designed_gross_area")]
    public int? ProjectDesignedGrossArea { get; init; }
    /// <summary>Field name for ProjectDesignedGrossArea, used in filters and order by clauses.</summary>
    /// <returns>"project_designed_gross_area"</returns>
    public static string ProjectDesignedGrossAreaField => "project_designed_gross_area";

    [JsonPropertyName("project_gross_net_factor")]
    public int? ProjectGrossNetFactor { get; init; }
    /// <summary>Field name for ProjectGrossNetFactor, used in filters and order by clauses.</summary>
    /// <returns>"project_gross_net_factor"</returns>
    public static string ProjectGrossNetFactorField => "project_gross_net_factor";

    [JsonPropertyName("room_level_gross_net_factor")]
    public int? RoomLevelGrossNetFactor { get; init; }
    /// <summary>Field name for RoomLevelGrossNetFactor, used in filters and order by clauses.</summary>
    /// <returns>"room_level_gross_net_factor"</returns>
    public static string RoomLevelGrossNetFactorField => "room_level_gross_net_factor";
}