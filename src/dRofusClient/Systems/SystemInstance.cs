namespace dRofusClient.Systems;

/// <summary>
/// Represents a system.
/// </summary>
public record SystemInstance : dRofusIdDto
{
    /// <summary>
    /// General: Description
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    public const string DescriptionField = "description";

    /// <summary>
    /// General: GUID
    /// </summary>
    [JsonPropertyName("guid")]
    public string? Guid { get; set; }
    public const string GuidField = "guid";

    /// <summary>
    /// General: Name
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    public const string NameField = "name";

    /// <summary>
    /// General: Number (read-only)
    /// </summary>
    [JsonPropertyName("number")]
    public string? Number { get; init; }
    public const string NumberField = "number";

    /// <summary>
    /// General: Serial Number
    /// </summary>
    [JsonPropertyName("run_no")]
    public string? SerialNumber { get; set; }
    public const string SerialNumberField = "run_no";

    /// <summary>
    /// General: Base Occurrence Id
    /// </summary>
    [JsonPropertyName("base_occurrence_id")]
    public int? SystemComponentId { get; init; }
    public const string SystemComponentIdField = "base_occurrence_id";

    /// <summary>
    /// Returns a copy of this system with read-only fields cleared.
    /// </summary>
    public SystemInstance ClearReadOnlyFields()
    {
        return this with
        {
            Number = null,
            SystemComponentId = null
        };
    }
}
