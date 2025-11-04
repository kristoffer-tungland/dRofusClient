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

    /// <summary>
    /// Sets a value in the database using a generated identifier based on the specified database ID.
    /// </summary>
    /// <remarks>The method generates a unique identifier by transforming the <paramref name="databaseId"/>
    /// into a specific format and uses it to store the provided <paramref name="value"/>. The identifier is constructed
    /// by splitting the database ID into two-digit segments and appending them to a base string.</remarks>
    /// <param name="databaseId">The ID of the database. Must be a non-negative integer.</param>
    /// <param name="value">The value to associate with the generated identifier. Cannot be <see langword="null"/>.</param>
    public void Set(int databaseId, object value)
    {
        var databaseIdParts = Enumerable.Range(0, databaseId.ToString("D2").Length / 2)
            .Select(i => databaseId.ToString("D2").Substring(i * 2, 2))
            .ToArray();

        var occurrenceDataId = $"occurrence_data_{string.Join("_", databaseIdParts)}";

        Set(occurrenceDataId, value);
    }
}
