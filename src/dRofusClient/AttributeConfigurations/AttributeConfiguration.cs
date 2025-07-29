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
public record AttributeConfiguration : dRofusDto
{
    [JsonPropertyName("available_to_users")]
    public bool? AvailableToUsers { get; init; }
    /// <summary>Field name for AvailableToUsers, used in filters and order by clauses.</summary>
    /// <returns>"available_to_users"</returns>
    public static string AvailableToUsersField => "available_to_users";

    [JsonPropertyName("is_default")]
    public bool? IsDefault { get; init; }
    /// <summary>Field name for IsDefault, used in filters and order by clauses.</summary>
    /// <returns>"is_default"</returns>
    public static string IsDefaultField => "is_default";

    [JsonPropertyName("name")]
    public string? Name { get; init; }
    /// <summary>Field name for Name, used in filters and order by clauses.</summary>
    /// <returns>"name"</returns>
    public static string NameField => "name";

    [JsonPropertyName("config_type")]
    public string? ConfigType { get; init; }
    /// <summary>Field name for ConfigType, used in filters and order by clauses.</summary>
    /// <returns>"config_type"</returns>
    public static string ConfigTypeField => "config_type";

    public AttributeConfigType ConfigTypeEnum => AttributeConfigTypeExtensions.FromRequest(ConfigType ?? string.Empty);

    [JsonPropertyName("applicable_to")]
    public List<string>? ApplicableTo { get; init; }
    /// <summary>Field name for ApplicableTo, used in filters and order by clauses.</summary>
    /// <returns>"applicable_to"</returns>
    public static string ApplicableToField => "applicable_to";

    public bool IsApplicableToAny =>
        ApplicableTo != null && ApplicableTo.Count > 0;

    [JsonPropertyName("elements")]
    public List<AttributeConfigurationElement>? Elements { get; init; }
    /// <summary>Field name for Elements, used in filters and order by clauses.</summary>
    /// <returns>"elements"</returns>
    public static string ElementsField => "elements";
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

public record AttributeConfigurationElement : dRofusDto
{
    [JsonPropertyName("configuration")]
    public int? Configuration { get; init; }
    /// <summary>Field name for Configuration, used in filters and order by clauses.</summary>
    /// <returns>"configuration"</returns>
    public static string ConfigurationField => "configuration";

    [JsonPropertyName("drofus_attribute_id")]
    public string? DrofusAttributeId { get; init; }
    /// <summary>Field name for DrofusAttributeId, used in filters and order by clauses.</summary>
    /// <returns>"drofus_attribute_id"</returns>
    public static string DrofusAttributeIdField => "drofus_attribute_id";

    [JsonPropertyName("drofus_attribute_label")]
    public string? DrofusAttributeLabel { get; init; }
    /// <summary>Field name for DrofusAttributeLabel, used in filters and order by clauses.</summary>
    /// <returns>"drofus_attribute_label"</returns>
    public static string DrofusAttributeLabelField => "drofus_attribute_label";

    [JsonPropertyName("direction")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AttributeConfigurationDirection Direction { get; init; }
    /// <summary>Field name for Direction, used in filters and order by clauses.</summary>
    /// <returns>"direction"</returns>
    public static string DirectionField => "direction";

    [JsonPropertyName("external_attribute_id")]
    public string? ExternalAttributeId { get; init; }
    /// <summary>Field name for ExternalAttributeId, used in filters and order by clauses.</summary>
    /// <returns>"external_attribute_id"</returns>
    public static string ExternalAttributeIdField => "external_attribute_id";

    [JsonPropertyName("external_attribute_label")]
    public string? ExternalAttributeLabel { get; init; }
    /// <summary>Field name for ExternalAttributeLabel, used in filters and order by clauses.</summary>
    /// <returns>"external_attribute_label"</returns>
    public static string ExternalAttributeLabelField => "external_attribute_label";
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