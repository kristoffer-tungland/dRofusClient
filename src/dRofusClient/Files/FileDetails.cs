namespace dRofusClient.Files;

/// <summary>
/// Represents file metadata as defined in the OpenAPI specification.
/// <para>See OpenAPI schema: File</para>
/// </summary>
public record FileDetails : dRofusIdDto
{
    /// <summary>
    /// General: Compressed size. The compressed size of the file in bytes.
    /// <para>Read-only.</para>
    /// </summary>
    [JsonPropertyName("compressed_size")]
    public int? CompressedSize { get; init; }
    public const string CompressedSizeField = "compressed_size";

    /// <summary>
    /// General: Uploaded. The date and time when the file was uploaded.
    /// <para>Read-only.</para>
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime? Created { get; init; }
    public const string CreatedField = "created";

    /// <summary>
    /// General: Description. The description of the file.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }
    public const string DescriptionField = "description";

    /// <summary>
    /// General: Last modified. The date and time when the file was last modified.
    /// <para>Read-only.</para>
    /// </summary>
    [JsonPropertyName("last_changed")]
    public DateTime? LastChanged { get; init; }
    public const string LastChangedField = "last_changed";

    /// <summary>
    /// General: Name. The name of the file.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
    public const string NameField = "name";

    /// <summary>
    /// General: Note. Additional notes for the file.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; init; }
    public const string NoteField = "note";

    /// <summary>
    /// General: Version. The revision/version number of the file.
    /// <para>Read-only.</para>
    /// </summary>
    [JsonPropertyName("revision")]
    public int? Revision { get; init; }
    public const string RevisionField = "revision";

    /// <summary>
    /// General: Size. The size of the file in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    public int? Size { get; init; }
    public const string SizeField = "size";

    /// <summary>
    /// General: File type. The type of the file.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }
    public const string TypeField = "type";

    /// <summary>
    /// The username of the user who created the file.
    /// </summary>
    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; init; }
    public const string CreatedByField = "created_by";

    /// <summary>
    /// The username of the user who last changed the file.
    /// </summary>
    [JsonPropertyName("last_changed_by")]
    public string? LastChangedBy { get; init; }
    public const string LastChangedByField = "last_changed_by";

    /// <summary>
    /// The responsibility associated with the file.
    /// </summary>
    [JsonPropertyName("responsibility")]
    public string? Responsibility { get; init; }
    public const string ResponsibilityField = "responsibility";
}
