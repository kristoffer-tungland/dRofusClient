namespace dRofusClient.PropertyMeta;

/// <summary>
/// Dto for dRofus options.
/// </summary>
/// <schema>
/// [
///   {
///     "id": "string",
///     "name": "string",
///     "propertyGroup": "string",
///     "dataType": "string",
///     "unit": "Undefined"
///   }
/// ]
/// </schema>
public record dRofusPropertyMeta : dRofusDto
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("propertyGroup")]
    public string? PropertyGroup { get; init; }

    [JsonPropertyName("dataType")]
    public required string DataType { get; init; }

    [JsonPropertyName("unit")]
    public string? Unit { get; init; }

    public string GetTitle()
    {
        return string.IsNullOrEmpty(PropertyGroup) ? Name : $"{PropertyGroup}: {Name}";
    }
}
