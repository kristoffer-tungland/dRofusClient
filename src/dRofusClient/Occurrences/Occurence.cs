namespace dRofusClient.Occurrences;

public record Occurence : dRofusDto
{
    [JsonPropertyName("addition_order_quantity")]
    public int? AdditionOrderQuantity { get; init; }
    /// <summary>Field name for AdditionOrderQuantity, used in filters and order by clauses.</summary>
    /// <returns>"addition_order_quantity"</returns>
    public static string AdditionOrderQuantityField => "addition_order_quantity";

    [JsonPropertyName("agreement_quantity")]
    public int? AgreementQuantity { get; init; }
    /// <summary>Field name for AgreementQuantity, used in filters and order by clauses.</summary>
    /// <returns>"agreement_quantity"</returns>
    public static string AgreementQuantityField => "agreement_quantity";

    [JsonPropertyName("agreement_quantity_option")]
    public int? AgreementQuantityOption { get; init; }
    /// <summary>Field name for AgreementQuantityOption, used in filters and order by clauses.</summary>
    /// <returns>"agreement_quantity_option"</returns>
    public static string AgreementQuantityOptionField => "agreement_quantity_option";

    [JsonPropertyName("article_id")]
    public int? ArticleId { get; init; }
    /// <summary>Field name for ArticleId, used in filters and order by clauses.</summary>
    /// <returns>"article_id"</returns>
    public static string ArticleIdField => "article_id";

    [JsonPropertyName("article_sub_article_id")]
    public int? ArticleSubArticleId { get; init; }
    /// <summary>Field name for ArticleSubArticleId, used in filters and order by clauses.</summary>
    /// <returns>"article_sub_article_id"</returns>
    public static string ArticleSubArticleIdField => "article_sub_article_id";

    [JsonPropertyName("category_id")]
    public int? CategoryId { get; init; }
    /// <summary>Field name for CategoryId, used in filters and order by clauses.</summary>
    /// <returns>"category_id"</returns>
    public static string CategoryIdField => "category_id";

    [JsonPropertyName("classification_number")]
    public string? ClassificationNumber { get; init; }
    /// <summary>Field name for ClassificationNumber, used in filters and order by clauses.</summary>
    /// <returns>"classification_number"</returns>
    public static string ClassificationNumberField => "classification_number";

    [JsonPropertyName("comment")]
    public string? Comment { get; init; }
    /// <summary>Field name for Comment, used in filters and order by clauses.</summary>
    /// <returns>"comment"</returns>
    public static string CommentField => "comment";

    [JsonPropertyName("equipment_list_type_id")]
    public int? EquipmentListTypeId { get; init; }
    /// <summary>Field name for EquipmentListTypeId, used in filters and order by clauses.</summary>
    /// <returns>"equipment_list_type_id"</returns>
    public static string EquipmentListTypeIdField => "equipment_list_type_id";

    [JsonPropertyName("existing_quantity")]
    public int? ExistingQuantity { get; init; }
    /// <summary>Field name for ExistingQuantity, used in filters and order by clauses.</summary>
    /// <returns>"existing_quantity"</returns>
    public static string ExistingQuantityField => "existing_quantity";

    [JsonPropertyName("net_quantity")]
    public int? NetQuantity { get; init; }
    /// <summary>Field name for NetQuantity, used in filters and order by clauses.</summary>
    /// <returns>"net_quantity"</returns>
    public static string NetQuantityField => "net_quantity";

    [JsonPropertyName("occurrence_name")]
    public string? OccurrenceName { get; init; }
    /// <summary>Field name for OccurrenceName, used in filters and order by clauses.</summary>
    /// <returns>"occurrence_name"</returns>
    public static string OccurrenceNameField => "occurrence_name";

    [JsonPropertyName("ordered_quantity")]
    public int? OrderedQuantity { get; init; }
    /// <summary>Field name for OrderedQuantity, used in filters and order by clauses.</summary>
    /// <returns>"ordered_quantity"</returns>
    public static string OrderedQuantityField => "ordered_quantity";

    [JsonPropertyName("product_id")]
    public int? ProductId { get; init; }
    /// <summary>Field name for ProductId, used in filters and order by clauses.</summary>
    /// <returns>"product_id"</returns>
    public static string ProductIdField => "product_id";

    [JsonPropertyName("quantity")]
    public int? Quantity { get; init; }
    /// <summary>Field name for Quantity, used in filters and order by clauses.</summary>
    /// <returns>"quantity"</returns>
    public static string QuantityField => "quantity";

    [JsonPropertyName("received_quantity")]
    public int? ReceivedQuantity { get; init; }
    /// <summary>Field name for ReceivedQuantity, used in filters and order by clauses.</summary>
    /// <returns>"received_quantity"</returns>
    public static string ReceivedQuantityField => "received_quantity";

    [JsonPropertyName("room_id")]
    public int? RoomId { get; init; }
    /// <summary>Field name for RoomId, used in filters and order by clauses.</summary>
    /// <returns>"room_id"</returns>
    public static string RoomIdField => "room_id";

    [JsonPropertyName("run_no")]
    public string? RunNo { get; init; }
    /// <summary>Field name for RunNo, used in filters and order by clauses.</summary>
    /// <returns>"run_no"</returns>
    public static string RunNoField => "run_no";

    [JsonPropertyName("tender_quantity")]
    public int? TenderQuantity { get; init; }
    /// <summary>Field name for TenderQuantity, used in filters and order by clauses.</summary>
    /// <returns>"tender_quantity"</returns>
    public static string TenderQuantityField => "tender_quantity";

    [JsonPropertyName("tender_quantity_option")]
    public int? TenderQuantityOption { get; init; }
    /// <summary>Field name for TenderQuantityOption, used in filters and order by clauses.</summary>
    /// <returns>"tender_quantity_option"</returns>
    public static string TenderQuantityOptionField => "tender_quantity_option";

    [JsonPropertyName("to_tender")]
    public bool? ToTender { get; init; }
    /// <summary>Field name for ToTender, used in filters and order by clauses.</summary>
    /// <returns>"to_tender"</returns>
    public static string ToTenderField => "to_tender";
}