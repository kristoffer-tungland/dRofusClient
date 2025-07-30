namespace dRofusClient.AttributeConfigurations;

//[
//  {
//    "elements": [
//      {
//        "id": 0,
//        "configuration": 0,
//        "drofus_attribute_id": "string",
//        "drofus_attribute_label": "string",
//        "direction": "string",
//        "external_attribute_id": "string",
//        "external_attribute_label": "string"
//      }
//    ],
//    "id": 0,
//    "applicable_to": [
//      "string"
//    ],
//    "available_to_users": true,
//    "config_type": "string",
//    "is_default": true,
//    "name": "string"
//  }
//]
public record AttributeConfiguration : dRofusIdDto
{
    [JsonPropertyName("available_to_users")]
    public bool? AvailableToUsers { get; init; }
    public const string AvailableToUsersField = "available_to_users";

    [JsonPropertyName("is_default")]
    public bool? IsDefault { get; init; }
    public const string IsDefaultField = "is_default";

    [JsonPropertyName("name")]
    public string? Name { get; init; }
    public const string NameField = "name";

    [JsonPropertyName("config_type")]
    public string? ConfigType { get; init; }
    public const string ConfigTypeField = "config_type";

    public AttributeConfigType ConfigTypeEnum => AttributeConfigTypeExtensions.FromRequest(ConfigType ?? string.Empty);

    [JsonPropertyName("applicable_to")]
    public List<string>? ApplicableTo { get; init; }
    public const string ApplicableToField = "applicable_to";

    [JsonPropertyName("elements")]
    public List<AttributeConfigurationElement>? Elements { get; init; }
    public const string ElementsField = "elements";
}


public enum AttributeConfigType
{
    Unknown,

    [JsonPropertyName("room")]
    Room,

    [JsonPropertyName("electrical_system")]
    ElectricalSystem,

    [JsonPropertyName("article")]
    Article,

    [JsonPropertyName("revit-occurrence")]
    RevitOccurrence,

    [JsonPropertyName("space")]
    Space,

    [JsonPropertyName("standardroom")]
    StandardRoom,

    [JsonPropertyName("duct_system")]
    DuctSystem,

    [JsonPropertyName("piping_system")]
    PipingSystem,

    [JsonPropertyName("piping_system_type")]
    PipingSystemType,

    [JsonPropertyName("standardroom_space")]
    StandardRoomSpace,

    [JsonPropertyName("duct_system_type")]
    DuctSystemType,

    [JsonPropertyName("revit-material")]
    RevitMaterial,

    [JsonPropertyName("revit-view")]
    RevitView,

    [JsonPropertyName("area")]
    Area
}

public static class AttributeConfigTypeExtensions
{
    public static string ToRequest(this AttributeConfigType attributeConfigType)
    {
        return attributeConfigType switch
        {
            AttributeConfigType.RevitOccurrence => "revit-occurrence",
            AttributeConfigType.Article => "article",
            AttributeConfigType.Space => "space",
            AttributeConfigType.ElectricalSystem => "electrical_system",
            AttributeConfigType.Room => "room",
            AttributeConfigType.StandardRoom => "standardroom",
            AttributeConfigType.DuctSystem => "duct_system",
            AttributeConfigType.PipingSystem => "piping_system",
            AttributeConfigType.PipingSystemType => "piping_system_type",
            AttributeConfigType.StandardRoomSpace => "standardroom_space",
            AttributeConfigType.DuctSystemType => "duct_system_type",
            AttributeConfigType.RevitMaterial => "revit-material",
            AttributeConfigType.RevitView => "revit-view",
            AttributeConfigType.Area => "area",
            _ => throw new ArgumentOutOfRangeException(nameof(attributeConfigType), attributeConfigType, null)
        };
    }

    public static AttributeConfigType FromRequest(string? request)
    {
        return request switch
        {
            "revit-occurrence" => AttributeConfigType.RevitOccurrence,
            "article" => AttributeConfigType.Article,
            "space" => AttributeConfigType.Space,
            "electrical_system" => AttributeConfigType.ElectricalSystem,
            "room" => AttributeConfigType.Room,
            "standardroom" => AttributeConfigType.StandardRoom,
            "duct_system" => AttributeConfigType.DuctSystem,
            "piping_system" => AttributeConfigType.PipingSystem,
            "piping_system_type" => AttributeConfigType.PipingSystemType,
            "standardroom_space" => AttributeConfigType.StandardRoomSpace,
            "duct_system_type" => AttributeConfigType.DuctSystemType,
            "revit-material" => AttributeConfigType.RevitMaterial,
            "revit-view" => AttributeConfigType.RevitView,
            "area" => AttributeConfigType.Area,
            _ => AttributeConfigType.Unknown
        };
    }
}

public record AttributeConfigurationElement : dRofusIdDto
{
    [JsonPropertyName("configuration")]
    public int? Configuration { get; init; }
    public const string ConfigurationField = "configuration";

    [JsonPropertyName("drofus_attribute_id")]
    public string? DrofusAttributeId { get; init; }
    public const string DrofusAttributeIdField = "drofus_attribute_id";

    [JsonPropertyName("drofus_attribute_label")]
    public string? DrofusAttributeLabel { get; init; }
    public const string DrofusAttributeLabelField = "drofus_attribute_label";

    [JsonPropertyName("direction")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AttributeConfigurationDirection Direction { get; init; }
    public const string DirectionField = "direction";

    [JsonPropertyName("external_attribute_id")]
    public string? ExternalAttributeId { get; init; }
    public const string ExternalAttributeIdField = "external_attribute_id";

    [JsonPropertyName("external_attribute_label")]
    public string? ExternalAttributeLabel { get; init; }
    public const string ExternalAttributeLabelField = "external_attribute_label";
}

public enum AttributeConfigurationDirection
{
    Key = 0,
    ToExternalApplication = 1,
    ToDrofus = 2,
}

public static class dRofusAttributeConfigurationDtoExtensions
{
    public static AttributeConfigurationElement GetIdentifierElement(
        this AttributeConfiguration attributeConfiguration)
    {
        foreach (var element in attributeConfiguration.Elements ?? Enumerable.Empty<AttributeConfigurationElement>())
        {
            if (element.Direction == AttributeConfigurationDirection.Key)
            {
                return element;
            }
        }

        throw new InvalidOperationException("No identifier element found in attribute configuration");
    }
}