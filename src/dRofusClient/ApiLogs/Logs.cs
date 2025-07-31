namespace dRofusClient.ApiLogs;

/// <summary>
/// Base log entry as defined in the OpenAPI specification.
/// </summary>
public record Log : dRofusDto
{
    /// <summary>
    /// General: Event. The event that occurred (e.g., update, create, delete).
    /// </summary>
    [JsonPropertyName("action")]
    public string? Action { get; init; }
    public const string ActionField = "action";

    /// <summary>
    /// General: Field. The field that was changed, if applicable.
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; init; }
    public const string FieldField = "field";

    /// <summary>
    /// General: Change source. The origin of the change (e.g., user, system, import).
    /// </summary>
    [JsonPropertyName("change_source")]
    public string? ChangeSource { get; init; }
    public const string ChangeSourceField = "change_source";

    /// <summary>
    /// General: New value. The value after the change.
    /// </summary>
    [JsonPropertyName("new_value")]
    public string? NewValue { get; init; }
    public const string NewValueField = "new_value";

    /// <summary>
    /// General: Log Note. An optional note or comment about the change.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; init; }
    public const string NoteField = "note";

    /// <summary>
    /// General: Old value. The value before the change.
    /// </summary>
    [JsonPropertyName("old_value")]
    public string? OldValue { get; init; }
    public const string OldValueField = "old_value";

    /// <summary>
    /// General: Time. The timestamp when the change occurred.
    /// </summary>
    [JsonPropertyName("time")]
    public DateTime? Time { get; init; }
    public const string TimeField = "time";

    /// <summary>
    /// General: Username. The username of the user who performed the action.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; init; }
    public const string UsernameField = "username";
}

/// <summary>
/// Log entry for Items. Contains log information specific to Item entities.
/// <para>See OpenAPI schema: ItemLog</para>
/// </summary>
public record ItemLog : Log
{
    /// <summary>
    /// General: Item ID. The unique identifier of the Item associated with this log entry.
    /// </summary>
    [JsonPropertyName("article_id")]
    public int? ArticleId { get; init; }
    public const string ArticleIdField = "article_id";
}

/// <summary>
/// Log entry for Occurrences. Contains log information specific to Occurrence entities.
/// <para>See OpenAPI schema: OccurrenceLog</para>
/// </summary>
public record OccurrenceLog : Log
{
    /// <summary>
    /// General: Occurrence ID. The unique identifier of the Occurrence associated with this log entry.
    /// </summary>
    [JsonPropertyName("occurrence_id")]
    public int? OccurrenceId { get; init; }
    public const string OccurrenceIdField = "occurrence_id";
}

/// <summary>
/// Log entry for Systems. Contains log information specific to System entities.
/// <para>See OpenAPI schema: SystemLog</para>
/// </summary>
public record SystemLog : Log
{
    /// <summary>
    /// General: System ID. The unique identifier of the System associated with this log entry.
    /// </summary>
    [JsonPropertyName("system_id")]
    public int? SystemId { get; init; }
    public const string SystemIdField = "system_id";
}

/// <summary>
/// Log entry for Rooms. Contains log information specific to Room entities.
/// <para>See OpenAPI schema: RoomLog</para>
/// </summary>
public record RoomLog : Log
{
    /// <summary>
    /// General: Room ID. The unique identifier of the Room associated with this log entry.
    /// </summary>
    [JsonPropertyName("room_id")]
    public int? RoomId { get; init; }
    public const string RoomIdField = "room_id";
}
