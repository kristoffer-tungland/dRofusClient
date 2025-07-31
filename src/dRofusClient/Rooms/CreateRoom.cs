namespace dRofusClient.Rooms;

/// <summary>
/// Request body for creating a room. Only common fields from the OpenAPI schema are included.
/// </summary>
public record CreateRoom : dRofusDto
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("architect_no")]
    public string? ArchitectNo { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("programmed_area")]
    public double? ProgrammedArea { get; init; }

    [JsonPropertyName("room_function_id")]
    public int? RoomFunctionId { get; init; }

    [JsonPropertyName("user_room_no")]
    public string? UserRoomNo { get; init; }
}
