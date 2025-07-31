namespace dRofusClient.Files;

/// <summary>
/// Represents image metadata.
/// </summary>
public record Image : dRofusIdDto
{
    [JsonPropertyName("created")]
    public DateTime? Created { get; init; }
    public const string CreatedField = "created";

    [JsonPropertyName("note")]
    public string? Note { get; init; }
    public const string NoteField = "note";

    [JsonPropertyName("position")]
    public int? Position { get; init; }
    public const string PositionField = "position";
}
