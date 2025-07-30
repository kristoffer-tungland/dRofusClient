using dRofusClient.ItemGroups;

namespace dRofusClient.Items;

/// <summary>
/// CreateItem request body for dRofus API
///
/// level_id (required): General: Item Group ID
/// name (required): General: Name
/// bim_id: General: BIM ID
/// bip: General: ASE
/// note: General: Note
/// parent_id: General: Parent ID
/// price_reference: General: Reference
/// serial_no: General: Serial Number (maxLength: 10)
/// to_be_drawn: General: To be modeled
///
/// See dRofus OpenAPI for details.
/// </summary>
public record CreateItem : dRofusDto
{
    /// <summary>
    /// General: Item Group ID (required)
    /// </summary>
    [JsonPropertyName("level_id")]
    public required int LevelId { get; init; }

    /// <summary>
    /// General: Name (required)
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// General: BIM ID
    /// </summary>
    [JsonPropertyName("bim_id")]
    public string? BimId { get; init; }

    /// <summary>
    /// General: ASE
    /// </summary>
    [JsonPropertyName("bip")]
    public bool? Bip { get; init; }

    /// <summary>
    /// General: Note
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; init; }

    /// <summary>
    /// General: Parent ID
    /// </summary>
    [JsonPropertyName("parent_id")]
    public int? ParentId { get; init; }

    /// <summary>
    /// General: Reference
    /// </summary>
    [JsonPropertyName("price_reference")]
    public string? PriceReference { get; init; }

    /// <summary>
    /// General: Serial Number (maxLength: 10)
    /// </summary>
    [JsonPropertyName("serial_no")]
    public string? SerialNo { get; init; }

    /// <summary>
    /// General: To be modeled
    /// </summary>
    [JsonPropertyName("to_be_drawn")]
    public bool? ToBeDrawn { get; init; }

    public static CreateItem With(ItemGroup itemGroup, string name)
    {
        return new CreateItem
        {
            LevelId = itemGroup.GetId(),
            Name = name
        };
    }
}
