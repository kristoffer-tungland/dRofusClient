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
public record dRofusAttributeConfiguration : dRofusDto
{
    public int? Id { get; init; }
    public bool? AvailableToUsers { get; init; }
    public bool? IsDefault { get; init; }
    public string? Name { get; init; }
    public string? ConfigType { get; init; }
    public List<string>? ApplicableTo { get; init; }
    public List<dRofusAttributeConfigurationElement>? Elements { get; init; }
}

public record dRofusAttributeConfigurationElement
{
    public int? Id { get; init; }
    public int? Configuration { get; init; }
    public string? DrofusAttributeId { get; init; }
    public string? DrofusAttributeLabel { get; init; }
    public string? Direction { get; init; }
    public string? ExternalAttributeId { get; init; }
    public string? ExternalAttributeLabel { get; init; }
}