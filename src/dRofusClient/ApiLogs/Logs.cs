namespace dRofusClient.ApiLogs;

/// <summary>
/// Base log entry as defined in the OpenAPI specification.
/// </summary>
public record Log : dRofusDto
{
    [JsonPropertyName("action")]
    public string? Action { get; init; }
    public const string ActionField = "action";

    [JsonPropertyName("field")]
    public string? Field { get; init; }
    public const string FieldField = "field";

    [JsonPropertyName("change_source")]
    public string? ChangeSource { get; init; }
    public const string ChangeSourceField = "change_source";

    [JsonPropertyName("new_value")]
    public string? NewValue { get; init; }
    public const string NewValueField = "new_value";

    [JsonPropertyName("note")]
    public string? Note { get; init; }
    public const string NoteField = "note";

    [JsonPropertyName("old_value")]
    public string? OldValue { get; init; }
    public const string OldValueField = "old_value";

    [JsonPropertyName("time")]
    public DateTime? Time { get; init; }
    public const string TimeField = "time";

    [JsonPropertyName("username")]
    public string? Username { get; init; }
    public const string UsernameField = "username";
}

/// <summary>
/// Log entry for Items.
/// </summary>
public record ItemLog : Log
{
    [JsonPropertyName("article_id")]
    public int? ArticleId { get; init; }
    public const string ArticleIdField = "article_id";
}

/// <summary>
/// Log entry for Occurrences.
/// </summary>
public record OccurrenceLog : Log
{
    [JsonPropertyName("occurrence_id")]
    public int? OccurrenceId { get; init; }
    public const string OccurrenceIdField = "occurrence_id";
}

/// <summary>
/// Log entry for Systems.
/// </summary>
public record SystemLog : Log
{
    [JsonPropertyName("system_id")]
    public int? SystemId { get; init; }
    public const string SystemIdField = "system_id";
}

/// <summary>
/// Log entry for Rooms.
/// </summary>
public record RoomLog : Log
{
    [JsonPropertyName("room_id")]
    public int? RoomId { get; init; }
    public const string RoomIdField = "room_id";
}
