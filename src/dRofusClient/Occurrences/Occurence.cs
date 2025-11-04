namespace dRofusClient.Occurrences;

public record Occurence : dRofusIdDto
{
    /// <summary>
    /// Quantity for addition order (read-only).
    /// </summary>
    [JsonPropertyName("addition_order_quantity")]
    public int? AdditionOrderQuantity { get; init; }
    /// <summary>Field name for AdditionOrderQuantity, used in filters and order by clauses.</summary>
    public const string AdditionOrderQuantityField = "addition_order_quantity";

    /// <summary>
    /// Quantity for agreement (read-only).
    /// </summary>
    [JsonPropertyName("agreement_quantity")]
    public int? AgreementQuantity { get; init; }
    /// <summary>Field name for AgreementQuantity, used in filters and order by clauses.</summary>
    public const string AgreementQuantityField = "agreement_quantity";

    /// <summary>
    /// Quantity for agreement option (read-only).
    /// </summary>
    [JsonPropertyName("agreement_quantity_option")]
    public int? AgreementQuantityOption { get; init; }
    /// <summary>Field name for AgreementQuantityOption, used in filters and order by clauses.</summary>
    public const string AgreementQuantityOptionField = "agreement_quantity_option";

    /// <summary>
    /// Item ID (article_id).
    /// </summary>
    [JsonPropertyName("article_id")]
    public int? ArticleId { get; set; }
    /// <summary>Field name for ArticleId, used in filters and order by clauses.</summary>
    public const string ArticleIdField = "article_id";

    /// <summary>
    /// Sub Item ID (article_sub_article_id).
    /// </summary>
    [JsonPropertyName("article_sub_article_id")]
    public int? ArticleSubArticleId { get; set; }
    /// <summary>Field name for ArticleSubArticleId, used in filters and order by clauses.</summary>
    public const string ArticleSubArticleIdField = "article_sub_article_id";

    /// <summary>
    /// Category ID.
    /// </summary>
    [JsonPropertyName("category_id")]
    public int? CategoryId { get; set; }
    /// <summary>Field name for CategoryId, used in filters and order by clauses.</summary>
    public const string CategoryIdField = "category_id";

    /// <summary>
    /// Classification number (read-only).
    /// </summary>
    [JsonPropertyName("classification_number")]
    public string? ClassificationNumber { get; init; }
    /// <summary>Field name for ClassificationNumber, used in filters and order by clauses.</summary>
    public const string ClassificationNumberField = "classification_number";

    /// <summary>
    /// Comment for the occurrence.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
    /// <summary>Field name for Comment, used in filters and order by clauses.</summary>
    public const string CommentField = "comment";

    /// <summary>
    /// Item List Type ID (equipment_list_type_id).
    /// </summary>
    [JsonPropertyName("equipment_list_type_id")]
    public int? EquipmentListTypeId { get; set; }
    /// <summary>Field name for EquipmentListTypeId, used in filters and order by clauses.</summary>
    public const string EquipmentListTypeIdField = "equipment_list_type_id";

    /// <summary>
    /// Existing quantity.
    /// </summary>
    [JsonPropertyName("existing_quantity")]
    public int? ExistingQuantity { get; set; }
    /// <summary>Field name for ExistingQuantity, used in filters and order by clauses.</summary>
    public const string ExistingQuantityField = "existing_quantity";

    /// <summary>
    /// Net quantity (read-only).
    /// </summary>
    [JsonPropertyName("net_quantity")]
    public int? NetQuantity { get; init; }
    /// <summary>Field name for NetQuantity, used in filters and order by clauses.</summary>
    public const string NetQuantityField = "net_quantity";

    /// <summary>
    /// Name of the occurrence.
    /// </summary>
    [JsonPropertyName("occurrence_name")]
    public string? OccurrenceName { get; set; }
    /// <summary>Field name for OccurrenceName, used in filters and order by clauses.</summary>
    public const string OccurrenceNameField = "occurrence_name";

    /// <summary>
    /// Ordered quantity (read-only).
    /// </summary>
    [JsonPropertyName("ordered_quantity")]
    public int? OrderedQuantity { get; init; }
    /// <summary>Field name for OrderedQuantity, used in filters and order by clauses.</summary>
    public const string OrderedQuantityField = "ordered_quantity";

    /// <summary>
    /// Product ID.
    /// </summary>
    [JsonPropertyName("product_id")]
    public int? ProductId { get; set; }
    /// <summary>Field name for ProductId, used in filters and order by clauses.</summary>
    public const string ProductIdField = "product_id";

    /// <summary>
    /// Quantity for the occurrence.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }
    /// <summary>Field name for Quantity, used in filters and order by clauses.</summary>
    public const string QuantityField = "quantity";

    /// <summary>
    /// Received quantity (read-only).
    /// </summary>
    [JsonPropertyName("received_quantity")]
    public int? ReceivedQuantity { get; init; }
    /// <summary>Field name for ReceivedQuantity, used in filters and order by clauses.</summary>
    public const string ReceivedQuantityField = "received_quantity";

    /// <summary>
    /// Room ID.
    /// </summary>
    [JsonPropertyName("room_id")]
    public int? RoomId { get; set; }
    /// <summary>Field name for RoomId, used in filters and order by clauses.</summary>
    public const string RoomIdField = "room_id";

    /// <summary>
    /// Serial number (run_no).
    /// </summary>
    [JsonPropertyName("run_no")]
    public string? RunNo { get; set; }
    /// <summary>Field name for RunNo, used in filters and order by clauses.</summary>
    public const string RunNoField = "run_no";

    /// <summary>
    /// Tender quantity (read-only).
    /// </summary>
    [JsonPropertyName("tender_quantity")]
    public int? TenderQuantity { get; init; }
    /// <summary>Field name for TenderQuantity, used in filters and order by clauses.</summary>
    public const string TenderQuantityField = "tender_quantity";

    /// <summary>
    /// Tender quantity option (read-only).
    /// </summary>
    [JsonPropertyName("tender_quantity_option")]
    public int? TenderQuantityOption { get; init; }
    /// <summary>Field name for TenderQuantityOption, used in filters and order by clauses.</summary>
    public const string TenderQuantityOptionField = "tender_quantity_option";

    /// <summary>
    /// Indicates if the occurrence is in tender (read-only).
    /// </summary>
    [JsonPropertyName("to_tender")]
    public bool? ToTender { get; init; }
    /// <summary>Field name for ToTender, used in filters and order by clauses.</summary>
    public const string ToTenderField = "to_tender";

    /// <summary>
    /// Parent occurrence ID (read-only).
    /// </summary>
    [JsonPropertyName("parent_occurrence_id")]
    public int? ParentOccurrenceId { get; init; }
    /// <summary>Field name for ParentOccurrenceId, used in filters and order by clauses.</summary>
    public const string ParentOccurrenceIdField = "parent_occurrence_id";

    /// <summary>
    /// Project ID (read-only).
    /// </summary>
    [JsonPropertyName("project_id")]
    public string? ProjectId { get; init; }
    /// <summary>Field name for ProjectId, used in filters and order by clauses.</summary>
    public const string ProjectIdField = "project_id";

    /// <summary>
    /// Article sub category ID (read-only).
    /// </summary>
    [JsonPropertyName("article_sub_category_id")]
    public int? ArticleSubCategoryId { get; init; }
    /// <summary>Field name for ArticleSubCategoryId, used in filters and order by clauses.</summary>
    public const string ArticleSubCategoryIdField = "article_sub_category_id";

    /// <summary>
    /// Responsibility for the occurrence (read-only).
    /// </summary>
    [JsonPropertyName("occurrence_responsibility")]
    public string? OccurrenceResponsibility { get; init; }
    /// <summary>Field name for OccurrenceResponsibility, used in filters and order by clauses.</summary>
    public const string OccurrenceResponsibilityField = "occurrence_responsibility";

    /// <summary>
    /// Owner of the occurrence (read-only).
    /// </summary>
    [JsonPropertyName("owner")]
    public string? Owner { get; init; }
    /// <summary>Field name for Owner, used in filters and order by clauses.</summary>
    public const string OwnerField = "owner";

    /// <summary>
    /// Priority of the occurrence (read-only).
    /// </summary>
    [JsonPropertyName("priority")]
    public string? Priority { get; init; }
    /// <summary>Field name for Priority, used in filters and order by clauses.</summary>
    public const string PriorityField = "priority";

    /// <summary>
    /// Agreement ID (read-only).
    /// </summary>
    [JsonPropertyName("agreement_id")]
    public int? AgreementId { get; init; }
    /// <summary>Field name for AgreementId, used in filters and order by clauses.</summary>
    public const string AgreementIdField = "agreement_id";

    /// <summary>
    /// Order ID (read-only).
    /// </summary>
    [JsonPropertyName("order_id")]
    public int? OrderId { get; init; }
    /// <summary>Field name for OrderId, used in filters and order by clauses.</summary>
    public const string OrderIdField = "order_id";

    /// <summary>
    /// Tender ID (read-only).
    /// </summary>
    [JsonPropertyName("tender_id")]
    public int? TenderId { get; init; }
    /// <summary>Field name for TenderId, used in filters and order by clauses.</summary>
    public const string TenderIdField = "tender_id";

    /// <summary>
    /// Responsibility (read-only).
    /// </summary>
    /// <returns>String representing the responsibility of the occurrence.</returns>
    [JsonPropertyName("responsibility")]
    public string? Responsibility { get; init; }
    /// <summary>Field name for Responsibility, used in filters and order by clauses.</summary>
    public const string ResponsibilityField = "responsibility";

    public Occurence ClearReadOnlyFields()
    {
        return this with
        {
            AdditionOrderQuantity = null,
            AgreementQuantity = null,
            AgreementQuantityOption = null,
            ClassificationNumber = null,
            NetQuantity = null,
            OrderedQuantity = null,
            ReceivedQuantity = null,
            TenderQuantity = null,
            TenderQuantityOption = null,
            ToTender = null,
            ParentOccurrenceId = null,
            ProjectId = null,
            ArticleSubCategoryId = null,
            OccurrenceResponsibility = null,
            Owner = null,
            Priority = null,
            AgreementId = null,
            OrderId = null,
            TenderId = null,
            Responsibility = null
        };
    }
}