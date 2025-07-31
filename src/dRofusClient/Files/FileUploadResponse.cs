namespace dRofusClient.Files;

/// <summary>
/// Response returned when uploading a file.
/// </summary>
public record FileUploadResponse : dRofusDto
{
    [JsonPropertyName("fileId")]
    public int FileId { get; init; }
    public const string FileIdField = "fileId";

    [JsonPropertyName("fileName")]
    public string? FileName { get; init; }
    public const string FileNameField = "fileName";

    [JsonPropertyName("size")]
    public long Size { get; init; }
    public const string SizeField = "size";
}
