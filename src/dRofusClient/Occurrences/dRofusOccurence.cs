namespace dRofusClient.Occurrences;

public record dRofusOccurence : dRofusDto
{
    [JsonProperty("id")]
    public int? Id { get; init; }

    [JsonProperty("addition_order_quantity")]
    public int? AdditionOrderQuantity { get; init; }

    [JsonProperty("agreement_quantity")]
    public int? AgreementQuantity { get; init; }

    [JsonProperty("agreement_quantity_option")]
    public int? AgreementQuantityOption { get; init; }

    [JsonProperty("article_id")]
    public int? ArticleId { get; init; }

    [JsonProperty("article_sub_article_id")]
    public int? ArticleSubArticleId { get; init; }

    [JsonProperty("category_id")]
    public int? CategoryId { get; init; }

    [JsonProperty("classification_number")]
    public string? ClassificationNumber { get; init; }

    [JsonProperty("comment")]
    public string? Comment { get; init; }

    [JsonProperty("equipment_list_type_id")]
    public int? EquipmentListTypeId { get; init; }

    [JsonProperty("existing_quantity")]
    public int? ExistingQuantity { get; init; }

    [JsonProperty("net_quantity")]
    public int? NetQuantity { get; init; }

    [JsonProperty("occurrence_name")]
    public string? OccurrenceName { get; init; }

    [JsonProperty("ordered_quantity")]
    public int? OrderedQuantity { get; init; }

    [JsonProperty("product_id")]
    public int? ProductId { get; init; }

    [JsonProperty("quantity")]
    public int? Quantity { get; init; }

    [JsonProperty("received_quantity")]
    public int? ReceivedQuantity { get; init; }

    [JsonProperty("room_id")]
    public int? RoomId { get; init; }

    [JsonProperty("run_no")]
    public string? RunNo { get; init; }

    [JsonProperty("tender_quantity")]
    public int? TenderQuantity { get; init; }

    [JsonProperty("tender_quantity_option")]
    public int? TenderQuantityOption { get; init; }

    [JsonProperty("to_tender")]
    public bool? ToTender { get; init; }
}