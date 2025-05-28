using System.Text.Json;

namespace dRofusClient.Occurrences;

public static class dRofusClientOccurenceExtensions
{
    public static Task<List<dRofusOccurence>> GetOccurrencesAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusOccurence>(dRofusType.Occurrences.ToRequest(), options, cancellationToken);
    }

    public static Task<dRofusOccurence> CreateOccurrenceAsync(this IdRofusClient client, dRofusOccurence occurence, CancellationToken cancellationToken = default)
    {
        return client.PostAsync<dRofusOccurence>(dRofusType.Occurrences.ToRequest(), occurence.ToPostOption(), cancellationToken);
    }

    public static Task<dRofusOccurence> GetOccurrenceAsync(this IdRofusClient client, int id, dRofusFieldsOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusOccurence>(dRofusType.Occurrences.CombineToRequest(id), options, cancellationToken);
    }

    public static async Task<dRofusOccurence> UpdateOccurrenceAsync(this IdRofusClient client, dRofusOccurence occurence, CancellationToken cancellationToken = default)
    {
        var patchOptions = occurence.ToPatchOption();
        var copiedOccurence = JsonSerializer.Deserialize<dRofusOccurence>(JsonSerializer.Serialize(occurence))!;
        List<dRofusStatusPatchResult>? statusResults = null;
        
        if (patchOptions.StatusFields is not null)
        { 
            statusResults = await client.UpdateStatusesAsync(occurence.Id, patchOptions.StatusFields, cancellationToken);
            UpdateStatusesOnOccurence(copiedOccurence, statusResults);
        }

        if (patchOptions.Body is null || patchOptions.Body.Equals("{}"))
            return copiedOccurence;

        var occurenceResult = await client.PatchAsync<dRofusOccurence>(dRofusType.Occurrences.CombineToRequest(occurence.Id), patchOptions, cancellationToken);

        if (statusResults is not null)
            UpdateStatusesOnOccurence(occurenceResult, statusResults);

        return occurenceResult;
    }

    private static void UpdateStatusesOnOccurence(dRofusOccurence occurence, List<dRofusStatusPatchResult> statusResults)
    {
        occurence.AdditionalProperties ??= [];

        foreach (var result in statusResults)
        {
            var exsistingValue = occurence.GetProperty(result.PropertyName);

            switch (exsistingValue)
            {
                case string strValue:
                    occurence.AdditionalProperties[result.PropertyName] = result.Code ?? strValue;
                    break;
                case int intValue:
                    occurence.AdditionalProperties[result.PropertyName] = result.StatusId ?? intValue;
                    break;
                default:
                    occurence.AdditionalProperties[result.PropertyName] = result.Code ?? result.StatusId?.ToString() ?? string.Empty;
                    break;
            }
        }
    }

    private static async Task<List<dRofusStatusPatchResult>> UpdateStatusesAsync(this IdRofusClient client, int? id, List<JsonProperty> statusFields, CancellationToken cancellationToken)
    {
        var results = new List<dRofusStatusPatchResult>();

        if (id is null)
            throw new ArgumentNullException(nameof(id), "Occurrence ID cannot be null for status update.");

        foreach (var prop in statusFields)
        {
            var options = prop.ToStatusPatchOption();

            var result = await client.UpdateOccurrenceStatusAsync(id.Value, options, cancellationToken);
            results.Add(result);
        }

        return results;
    }

    public static async Task<dRofusStatusPatchResult> UpdateOccurrenceStatusAsync(this IdRofusClient client, int id, dRofusStatusPatchOptions options, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Occurrences.CombineToRequest(id, "statuses", options.StatusTypeId.ToString());
        var result = await client.PatchAsync<dRofusStatusPatchBody>(request, options, cancellationToken);

        return new dRofusStatusPatchResult
        {
            Code = result.Code,
            StatusId = result.StatusId,
            PropertyName = options.PropertyName
        };
    }

    public static Task DeleteOccurrenceAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync<dRofusOccurence>(dRofusType.Occurrences.CombineToRequest(id), null, cancellationToken);
    }
}
