// ReSharper disable InconsistentNaming


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
public record dRofusPropertyMeta(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("propertyGroup")] string PropertyGroup,
    [property: JsonPropertyName("dataType")] string DataType,
    [property: JsonPropertyName("unit")] string Unit
) : dRofusDto
{
    public string GetTitle()
    {
        return string.IsNullOrEmpty(PropertyGroup) ? Name : $"{PropertyGroup}: {Name}";
    }
}
