namespace dRofusClient.Occurrences;

public record dRofusOccurence : dRofusDto
{
    public int? Id { get; init; }
    public int? AdditionOrderQuantity { get; init; }
    public int? AgreementQuantity { get; init; }
    public int? AgreementQuantityOption { get; init; }
    public int? ArticleId { get; init; }
    public int? ArticleSubArticleId { get; init; }
    public int? CategoryId { get; init; }
    public string? ClassificationNumber { get; init; }
    public string? Comment { get; init; }
    public int? EquipmentListTypeId { get; init; }
    public int? ExistingQuantity { get; init; }
    public int? NetQuantity { get; init; }
    public string? OccurrenceName { get; init; }
    public int? OrderedQuantity { get; init; }
    public int? ProductId { get; init; }
    public int? Quantity { get; init; }
    public int? ReceivedQuantity { get; init; }
    public int? RoomId { get; init; }
    public string? RunNo { get; init; }
    public int? TenderQuantity { get; init; }
    public int? TenderQuantityOption { get; init; }
    public bool? ToTender { get; init; }
}