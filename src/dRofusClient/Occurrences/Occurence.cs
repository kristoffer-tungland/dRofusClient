namespace dRofusClient.Occurrences;

public record Occurence : dRofusIdDto
{
    [JsonPropertyName("addition_order_quantity")]
    public int? AdditionOrderQuantity { get; init; }
    public const string AdditionOrderQuantityField = "addition_order_quantity";

    [JsonPropertyName("agreement_quantity")]
    public int? AgreementQuantity { get; init; }
    public const string AgreementQuantityField = "agreement_quantity";

    [JsonPropertyName("agreement_quantity_option")]
    public int? AgreementQuantityOption { get; init; }
    public const string AgreementQuantityOptionField = "agreement_quantity_option";

    [JsonPropertyName("article_id")]
    public int? ArticleId { get; init; }
    public const string ArticleIdField = "article_id";

    [JsonPropertyName("article_sub_article_id")]
    public int? ArticleSubArticleId { get; init; }
    public const string ArticleSubArticleIdField = "article_sub_article_id";

    [JsonPropertyName("category_id")]
    public int? CategoryId { get; init; }
    public const string CategoryIdField = "category_id";

    [JsonPropertyName("classification_number")]
    public string? ClassificationNumber { get; init; }
    public const string ClassificationNumberField = "classification_number";

    [JsonPropertyName("comment")]
    public string? Comment { get; init; }
    public const string CommentField = "comment";

    [JsonPropertyName("equipment_list_type_id")]
    public int? EquipmentListTypeId { get; init; }
    public const string EquipmentListTypeIdField = "equipment_list_type_id";

    [JsonPropertyName("existing_quantity")]
    public int? ExistingQuantity { get; init; }
    public const string ExistingQuantityField = "existing_quantity";

    [JsonPropertyName("net_quantity")]
    public int? NetQuantity { get; init; }
    public const string NetQuantityField = "net_quantity";

    [JsonPropertyName("occurrence_name")]
    public string? OccurrenceName { get; init; }
    public const string OccurrenceNameField = "occurrence_name";

    [JsonPropertyName("ordered_quantity")]
    public int? OrderedQuantity { get; init; }
    public const string OrderedQuantityField = "ordered_quantity";

    [JsonPropertyName("product_id")]
    public int? ProductId { get; init; }
    public const string ProductIdField = "product_id";

    [JsonPropertyName("quantity")]
    public int? Quantity { get; init; }
    public const string QuantityField = "quantity";

    [JsonPropertyName("received_quantity")]
    public int? ReceivedQuantity { get; init; }
    public const string ReceivedQuantityField = "received_quantity";

    [JsonPropertyName("room_id")]
    public int? RoomId { get; init; }
    public const string RoomIdField = "room_id";

    [JsonPropertyName("run_no")]
    public string? RunNo { get; init; }
    public const string RunNoField = "run_no";

    [JsonPropertyName("tender_quantity")]
    public int? TenderQuantity { get; init; }
    public const string TenderQuantityField = "tender_quantity";

    [JsonPropertyName("tender_quantity_option")]
    public int? TenderQuantityOption { get; init; }
    public const string TenderQuantityOptionField = "tender_quantity_option";

    [JsonPropertyName("to_tender")]
    public bool? ToTender { get; init; }
    public const string ToTenderField = "to_tender";
}