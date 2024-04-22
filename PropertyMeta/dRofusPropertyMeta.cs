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
    [property: JsonProperty("id")] string Id,
    [property: JsonProperty("name")] string Name,
    [property: JsonProperty("propertyGroup")] string PropertyGroup,
    [property: JsonProperty("dataType")] string DataType,
    [property: JsonProperty("unit")] string Unit
) : dRofusDto
{
    public string GetTitle()
    {
        return string.IsNullOrEmpty(PropertyGroup) ? Name : $"{PropertyGroup}: {Name}";
    }
}