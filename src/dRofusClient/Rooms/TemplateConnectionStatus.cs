namespace dRofusClient.Rooms;

public enum TemplateConnectionType
{
    NotCreated,
    Unique,
    FromTemplate,
    DerivedFromTemplate
}

/// <summary>
/// Represents the connection status to a template.
/// </summary>
public record TemplateConnectionStatus : dRofusDto
{
    [JsonPropertyName("template_connection_type")]
    public TemplateConnectionType? TemplateConnectionType { get; init; }

    [JsonPropertyName("room_template_id")]
    public int? RoomTemplateId { get; init; }
}
