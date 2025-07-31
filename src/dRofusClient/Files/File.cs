namespace dRofusClient.Files;

/// <summary>
/// Represents file metadata as defined in the OpenAPI specification.
/// </summary>
public record File : dRofusIdDto
{
    [JsonPropertyName("compressed_size")]
    public int? CompressedSize { get; init; }
    public const string CompressedSizeField = "compressed_size";

    [JsonPropertyName("created")]
    public DateTime? Created { get; init; }
    public const string CreatedField = "created";

    [JsonPropertyName("description")]
    public string? Description { get; init; }
    public const string DescriptionField = "description";

    [JsonPropertyName("last_changed")]
    public DateTime? LastChanged { get; init; }
    public const string LastChangedField = "last_changed";

    [JsonPropertyName("name")]
    public string? Name { get; init; }
    public const string NameField = "name";

    [JsonPropertyName("note")]
    public string? Note { get; init; }
    public const string NoteField = "note";

    [JsonPropertyName("revision")]
    public int? Revision { get; init; }
    public const string RevisionField = "revision";

    [JsonPropertyName("size")]
    public int? Size { get; init; }
    public const string SizeField = "size";

    [JsonPropertyName("type")]
    public string? Type { get; init; }
    public const string TypeField = "type";
}
