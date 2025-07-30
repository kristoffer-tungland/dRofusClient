using dRofusClient.Items;

namespace dRofusClient.Occurrences;

/// <summary>
/// CreateOccurence request body for dRofus API
///
/// article_id (required): General: Item ID
/// category_id: General: Category ID
/// equipment_list_type_id: General: Item List Type ID
/// quantity: General: Quantity
/// room_id: General: Room ID
///
/// See dRofus OpenAPI for details.
/// </summary>
public record CreateOccurence : dRofusIdDto
{
    /// <summary>
    /// General: Item ID (required)
    /// </summary>
    [JsonPropertyName("article_id")]
    public required int ArticleId { get; init; }

    /// <summary>
    /// General: Category ID
    /// </summary>
    [JsonPropertyName("category_id")]
    public int? CategoryId { get; init; }

    /// <summary>
    /// General: Item List Type ID
    /// </summary>
    [JsonPropertyName("equipment_list_type_id")]
    public int? EquipmentListTypeId { get; init; }

    /// <summary>
    /// General: Quantity
    /// </summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; init; }

    /// <summary>
    /// General: Room ID
    /// </summary>
    [JsonPropertyName("room_id")]
    public int? RoomId { get; init; }

    public static CreateOccurence Of(Item item)
    {
        return new CreateOccurence
        {
            ArticleId = item.GetId(),
        };
    }
}
