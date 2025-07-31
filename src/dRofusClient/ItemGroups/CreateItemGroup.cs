namespace dRofusClient.ItemGroups;

/// <summary>
/// Request body for creating a new Item Group in dRofus.
/// </summary>
public record CreateItemGroup : dRofusDto
{
    /// <summary>
    /// General: Name (required)
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// General: Number (required)
    /// </summary>
    [JsonPropertyName("no")]
    public required string Number { get; init; }

    /// <summary>
    /// General: Description
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// General: Parent ID
    /// </summary>
    [JsonPropertyName("parent")]
    public int? Parent { get; init; }
}
