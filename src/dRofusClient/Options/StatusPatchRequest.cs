namespace dRofusClient.Options;

public record StatusPatchRequest : RequestBodyBase
{
    public int StatusTypeId => GetStatusTypeId(PropertyName);

    public required StatusPatchBody Body { get; init; }

    public override string Accept => "application/merge-patch+json";

    public required string PropertyName { get; init; }

    public override void AddParametersToRequest(List<RequestParameter> parameters)
    {

    }

    public override string GetBody()
    {
        return Json.Serialize(Body);
    }

    public static int GetStatusTypeId(string name)
    {
        // Extract the status type ID from the property name, for example ce156_id_or_parents.
        var statusTypeId = name.Split('_').FirstOrDefault()?.TrimStart('c', 'e');

        if (int.TryParse(statusTypeId, out var parsedStatusTypeId))
        {
            return parsedStatusTypeId;
        }

        // Alternative name format: occurrence_classification_156_classification_entry_id_id
        var parts = name.Split('_');
        if (parts.Length > 2)
        {
            statusTypeId = parts[2];
            if (int.TryParse(statusTypeId, out parsedStatusTypeId))
            {
                return parsedStatusTypeId;
            }
        }

        throw new ArgumentException($"Invalid status type ID in property name: {name}", nameof(name));
    }
}

/// <summary>
/// Patch body for updating dRofus status.
/// </summary>
/// <schema>
/// {
///  "code": "string",
///  "status_id": 0
/// }
/// </schema>
public record StatusPatchBody() : dRofusDto
{
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("status_id")]
    public int? StatusId { get; init; }
};

public record StatusPatchResult
{
    public int? StatusId { get; init; }
    public string? Code { get; init; }
    public required string PropertyName { get; init; }
};
