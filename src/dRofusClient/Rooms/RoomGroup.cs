namespace dRofusClient.Rooms;

/// <summary>
/// Represents a Room Group.
/// </summary>
public record RoomGroup : dRofusIdDto
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    public const string NameField = "name";

    [JsonPropertyName("description")]
    public string? Description { get; init; }
    public const string DescriptionField = "description";
}
