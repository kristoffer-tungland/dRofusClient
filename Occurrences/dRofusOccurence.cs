namespace dRofusClient.Occurrences;

public record dRofusOccurence(
    [property: JsonProperty("id")] int Id,
    [property: JsonProperty("addition_order_quantity")] int AdditionOrderQuantity,
    [property: JsonProperty("agreement_quantity")] int AgreementQuantity,
    [property: JsonProperty("agreement_quantity_option")] int AgreementQuantityOption,
    [property: JsonProperty("article_id")] int ArticleId,
    [property: JsonProperty("article_sub_article_id")] int ArticleSubArticleId,
    [property: JsonProperty("category_id")] int CategoryId,
    [property: JsonProperty("classification_number")] string ClassificationNumber,
    [property: JsonProperty("comment")] string Comment,
    [property: JsonProperty("equipment_list_type_id")] int EquipmentListTypeId,
    [property: JsonProperty("existing_quantity")] int ExistingQuantity,
    [property: JsonProperty("net_quantity")] int NetQuantity,
    [property: JsonProperty("occurrence_name")] string OccurrenceName,
    [property: JsonProperty("ordered_quantity")] int OrderedQuantity,
    [property: JsonProperty("product_id")] int ProductId,
    [property: JsonProperty("quantity")] int Quantity,
    [property: JsonProperty("received_quantity")] int ReceivedQuantity,
    [property: JsonProperty("room_id")] int RoomId,
    [property: JsonProperty("run_no")] string RunNo,
    [property: JsonProperty("tender_quantity")] int TenderQuantity,
    [property: JsonProperty("tender_quantity_option")] int TenderQuantityOption,
    [property: JsonProperty("to_tender")] bool ToTender
) : dRofusDto;