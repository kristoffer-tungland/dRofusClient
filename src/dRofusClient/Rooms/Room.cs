namespace dRofusClient.Rooms;

/// <summary>
/// Represents a Room. Only a subset of properties from the OpenAPI schema is included.
/// </summary>
public record Room : dRofusIdDto
{
    [JsonPropertyName("architect_no")]
    public string? ArchitectNo { get; init; }
    public const string ArchitectNoField = "architect_no";

    [JsonPropertyName("name")]
    public string? Name { get; set; }
    public const string NameField = "name";

    [JsonPropertyName("description")]
    public string? Description { get; set; }
    public const string DescriptionField = "description";

    [JsonPropertyName("note")]
    public string? Note { get; set; }
    public const string NoteField = "note";

    [JsonPropertyName("room_function_id")]
    public int? RoomFunctionId { get; set; }
    public const string RoomFunctionIdField = "room_function_id";

    [JsonPropertyName("user_room_no")]
    public string? UserRoomNo { get; set; }
    public const string UserRoomNoField = "user_room_no";

    [JsonPropertyName("created")]
    public DateTime? Created { get; init; }
    public const string CreatedField = "created";

    public Room ClearReadOnlyFields()
    {
        return this with { Created = null, ArchitectNo = null };
    }
}
