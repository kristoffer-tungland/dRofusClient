// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

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
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    /// <summary>Field name for Name, used in filters and order by clauses.</summary>
    /// <returns>"name"</returns>
    public static string NameField => "name";

    [JsonPropertyName("propertyGroup")]
    public string? PropertyGroup { get; init; }
    /// <summary>Field name for PropertyGroup, used in filters and order by clauses.</summary>
    /// <returns>"propertyGroup"</returns>
    public static string PropertyGroupField => "propertyGroup";

    [JsonPropertyName("dataType")]
    public required string DataType { get; init; }
    /// <summary>Field name for DataType, used in filters and order by clauses.</summary>
    /// <returns>"dataType"</returns>
    public static string DataTypeField => "dataType";

    [JsonPropertyName("unit")]
    public string? Unit { get; init; }
    /// <summary>Field name for Unit, used in filters and order by clauses.</summary>
    /// <returns>"unit"</returns>
    public static string UnitField => "unit";

    public string GetTitle()
    {
        return string.IsNullOrEmpty(PropertyGroup) ? Name : $"{PropertyGroup}: {Name}";
    }
}
