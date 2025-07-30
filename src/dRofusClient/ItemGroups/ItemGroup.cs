namespace dRofusClient.ItemGroups;

/// <summary>
/// Represents an Item Group in dRofus. See dRofus OpenAPI for property details.
/// </summary>
public record ItemGroup : dRofusIdDto
{
    /// <summary>
    /// General: Number
    /// </summary>
    [JsonPropertyName("no")]
    public string? Number { get; set; }
    public const string NumberField = "no";

    /// <summary>
    /// General: Name
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    public const string NameField = "name";

    /// <summary>
    /// General: Description
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    public const string DescriptionField = "description";

    /// <summary>
    /// General: Depth
    /// </summary>
    [JsonPropertyName("article_level_depth")]
    public int? ArticleLevelDepth { get; init; }
    public const string ArticleLevelDepthField = "article_level_depth";

    /// <summary>
    /// General: Full number
    /// </summary>
    [JsonPropertyName("full_no")]
    public string? FullNo { get; init; }
    public const string FullNoField = "full_no";

    /// <summary>
    /// General: Parent ID
    /// </summary>
    [JsonPropertyName("parent")]
    public int? Parent { get; set; }
    public const string ParentField = "parent";

    public ItemGroup ClearReadOnlyFields()
    {
        return this with
        {
            ArticleLevelDepth = null,
            FullNo = null,
        };
    }
}
