using dRofusClient.Occurrences;

namespace dRofusClient.Items;

public record Item : dRofusIdDto
{
    [JsonPropertyName("bim_id")]
    public string? BimId { get; set; }
    public const string BimIdField = "bim_id";

    [JsonPropertyName("bip")]
    public bool? Bip { get; set; }
    public const string BipField = "bip";

    [JsonPropertyName("child_number")]
    public string? ChildNumber { get; set; }
    public const string ChildNumberField = "child_number";

    [JsonPropertyName("classification_number")]
    public string? ClassificationNumber { get; init; }
    public const string ClassificationNumberField = "classification_number";

    [JsonPropertyName("level_id")]
    public int? LevelId { get; set; }
    public const string LevelIdField = "level_id";

    [JsonPropertyName("name")]
    public string? Name { get; set; }
    public const string NameField = "name";

    [JsonPropertyName("note")]
    public string? Note { get; set; }
    public const string NoteField = "note";

    [JsonPropertyName("number")]
    public string? Number { get; init; }
    public const string NumberField = "number";

    [JsonPropertyName("parent_id")]
    public int? ParentId { get; set; }
    public const string ParentIdField = "parent_id";

    [JsonPropertyName("price")]
    public double? Price { get; set; }
    public const string PriceField = "price";

    [JsonPropertyName("price_comment")]
    public string? PriceComment { get; set; }
    public const string PriceCommentField = "price_comment";

    [JsonPropertyName("price_date")]
    public string? PriceDate { get; init; }
    public const string PriceDateField = "price_date";

    [JsonPropertyName("price_reference")]
    public string? PriceReference { get; set; }
    public const string PriceReferenceField = "price_reference";

    [JsonPropertyName("run_no")]
    public string? RunNo { get; set; }
    public const string RunNoField = "run_no";

    [JsonPropertyName("serial_no")]
    public string? SerialNo { get; set; }
    public const string SerialNoField = "serial_no";

    [JsonPropertyName("to_be_drawn")]
    public bool? ToBeDrawn { get; set; }
    public const string ToBeDrawnField = "to_be_drawn";

    /// <summary>
    /// General: Responsibility
    /// </summary>
    /// <returns>Name of responsibility</returns>
    [JsonPropertyName("responsibility")]
    public string? Responsibility { get; init; }
    public const string ResponsibilityField = "responsibility";

    /// <summary>
    /// General: Budget group
    /// </summary>
    /// <returns>Name of budget group</returns>
    [JsonPropertyName("budget_group")]
    public string? BudgetGroup { get; set; }
    public const string BudgetGroupField = "budget_group";

    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; init; }
    public const string CreatedByField = "created_by";

    [JsonPropertyName("nkkn_group")]
    public string? NkknGroup { get; set; }
    public const string NkknGroupField = "nkkn_group";

    [JsonPropertyName("tender_id")]
    public int? TenderId { get; set; }
    public const string TenderIdField = "tender_id";

    [JsonPropertyName("number_name")]
    public string? NumberName { get; init; }
    public const string NumberNameField = "number_name";

    [JsonPropertyName("created")]
    public DateTime? Created { get; init; }
    public const string CreatedField = "created";
}
