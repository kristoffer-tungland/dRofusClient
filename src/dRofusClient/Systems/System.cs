namespace dRofusClient.Systems;

/// <summary>
/// Represents a system.
/// </summary>
public record System : dRofusIdDto
{
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    public const string DescriptionField = "description";

    [JsonPropertyName("guid")]
    public string? Guid { get; init; }
    public const string GuidField = "guid";

    [JsonPropertyName("name")]
    public string? Name { get; init; }
    public const string NameField = "name";

    [JsonPropertyName("number")]
    public string? Number { get; init; }
    public const string NumberField = "number";

    [JsonPropertyName("run_no")]
    public string? RunNo { get; init; }
    public const string RunNoField = "run_no";
}
