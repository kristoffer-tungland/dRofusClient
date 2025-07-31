namespace dRofusClient.Rooms;

/// <summary>
/// Represents a Room. Properties are mapped from the OpenAPI Room schema.
/// </summary>
public record Room : dRofusIdDto
{
    /// <summary>
    /// General: Room Number
    /// </summary>
    [JsonPropertyName("architect_no")]
    public string? RoomNumber { get; set; }
    public const string RoomNumberField = "architect_no";

    /// <summary>
    /// General: Room Name
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    public const string NameField = "name";

    /// <summary>
    /// General: Room Name Description
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    public const string DescriptionField = "description";

    /// <summary>
    /// General: Note
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }
    public const string NoteField = "note";

    /// <summary>
    /// General: Room Function ID
    /// </summary>
    [JsonPropertyName("room_function_id")]
    public int? RoomFunctionId { get; set; }
    public const string RoomFunctionIdField = "room_function_id";

    /// <summary>
    /// General: User Room Number
    /// </summary>
    [JsonPropertyName("user_room_no")]
    public string? UserRoomNumber { get; set; }
    public const string UserRoomNumberField = "user_room_no";

    /// <summary>
    /// Log History: Created (read-only)
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime? Created { get; init; }
    public const string CreatedField = "created";

    /// <summary>
    /// Areas and Measurements: Ceiling Height
    /// </summary>
    [JsonPropertyName("ceiling_height")]
    public double? CeilingHeight { get; set; }
    public const string CeilingHeightField = "ceiling_height";

    /// <summary>
    /// General: DELETED (read-only)
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool? Deleted { get; init; }
    public const string DeletedField = "deleted";

    /// <summary>
    /// Areas and Measurements: Designed Area
    /// </summary>
    [JsonPropertyName("designed_area")]
    public double? DesignedArea { get; set; }
    public const string DesignedAreaField = "designed_area";

    /// <summary>
    /// General: Name on Drawing
    /// </summary>
    [JsonPropertyName("drawing_name")]
    public string? NameOnDrawing { get; set; }
    public const string NameOnDrawingField = "drawing_name";

    /// <summary>
    /// General: Additional Number
    /// </summary>
    [JsonPropertyName("drawing_no")]
    public string? AdditionalNumber { get; set; }
    public const string AdditionalNumberField = "drawing_no";

    /// <summary>
    /// Areas and Measurements: Perimeter
    /// </summary>
    [JsonPropertyName("perimeter")]
    public double? Perimeter { get; set; }
    public const string PerimeterField = "perimeter";

    /// <summary>
    /// Areas and Measurements: Programmed Area
    /// </summary>
    [JsonPropertyName("programmed_area")]
    public double? ProgrammedArea { get; set; }
    public const string ProgrammedAreaField = "programmed_area";

    /// <summary>
    /// General: Project ID (read-only)
    /// </summary>
    [JsonPropertyName("project_id")]
    public string? ProjectId { get; init; }
    public const string ProjectIdField = "project_id";

    /// <summary>
    /// General: Room Function # (read-only)
    /// </summary>
    [JsonPropertyName("room_func_no")]
    public string? RoomFunctionNumber { get; init; }
    public const string RoomFunctionNumberField = "room_func_no";

    /// <summary>
    /// AdditionalProperties: full_name (read-only)
    /// </summary>
    [JsonPropertyName("full_name")]
    public string? FullName { get; init; }
    public const string FullNameField = "full_name";

    /// <summary>
    /// Indicates if the room is a standard room according to RFP (Request for Proposal).
    /// </summary>
    [JsonPropertyName("rfp_standard_room")]
    public string? RfpStandardRoom { get; init; }
    public const string RfpStandardRoomField = "rfp_standard_room";

    /// <summary>
    /// The user who created the room. This is a read-only field.
    /// </summary>
    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; init; }
    public const string CreatedByField = "created_by";

    public Room ClearReadOnlyFields()
    {
        return this with
        {
            Created = null,
            Deleted = null,
            ProjectId = null,
            RoomFunctionNumber = null,
            FullName = null,
            RfpStandardRoom = null,
            CreatedBy = null,
        };
    }
}
