namespace dRofusClient.Rooms;

/// <summary>
/// Represents a request body for updating a room group assignment.
/// </summary>
public record RoomGroupUpdate : dRofusDto
{
    [JsonPropertyName("room_group_id")]
    public int? RoomGroupId { get; init; }

    [JsonPropertyName("room_group_name")]
    public string? RoomGroupName { get; init; }
}
