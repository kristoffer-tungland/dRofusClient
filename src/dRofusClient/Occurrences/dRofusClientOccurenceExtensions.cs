using System.Text.Json;

namespace dRofusClient.Occurrences;

/// <summary>
/// Extension methods for working with Occurrence resources using the IdRofusClient.
/// </summary>
public static class dRofusClientOccurenceExtensions
{
    /// <summary>
    /// Retrieves a list of occurrences matching the specified query.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="query">The query parameters for filtering and paging.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of <see cref="Occurence"/> objects.</returns>
    public static Task<List<Occurence>> GetOccurrencesAsync(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<Occurence>(dRofusType.Occurrences.ToRequest(), query, cancellationToken);
    }

    /// <summary>
    /// Creates a new occurrence with the specified parameters.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="articleId">The article ID for the occurrence.</param>
    /// <param name="categoryId">The category ID for the occurrence (optional).</param>
    /// <param name="equipmentListTypeId">The equipment list type ID (optional).</param>
    /// <param name="quantity">The quantity for the occurrence (optional).</param>
    /// <param name="roomId">The room ID for the occurrence (optional).</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created <see cref="Occurence"/> object.</returns>
    public static Task<Occurence> CreateOccurrenceAsync(this IdRofusClient client, CreateOccurence occurenceToCreate, CancellationToken cancellationToken = default)
    {
        return client.PostAsync<Occurence>(dRofusType.Occurrences.ToRequest(), occurenceToCreate.ToPostRequest(), cancellationToken);
    }

    /// <summary>
    /// Retrieves a single occurrence by its ID.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="id">The ID of the occurrence.</param>
    /// <param name="query">Optional query parameters for field selection.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The <see cref="Occurence"/> object with the specified ID.</returns>
    public static Task<Occurence> GetOccurrenceAsync(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<Occurence>(dRofusType.Occurrences.CombineToRequest(id), query, cancellationToken);
    }

    /// <summary>
    /// Updates an existing occurrence. If status fields are present, updates statuses as well.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="occurence">The occurrence to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated <see cref="Occurence"/> object.</returns>
    public static async Task<Occurence> UpdateOccurrenceAsync(this IdRofusClient client, Occurence occurence, CancellationToken cancellationToken = default)
    {
        var patchOptions = occurence.ToPatchRequest();

        Occurence? occurenceResult = null;

        if (patchOptions.Body is not null && patchOptions.Body.Equals("{}") == false)
            occurenceResult = await client.PatchAsync<Occurence>(dRofusType.Occurrences.CombineToRequest(occurence.Id), patchOptions, cancellationToken);

        occurenceResult ??= occurence with { Id = occurence.Id, AdditionalProperties = occurence.AdditionalProperties };

        if (patchOptions.StatusFields is not null)
        {
            var statusResults = await client.UpdateStatusesAsync(occurence.Id, patchOptions.StatusFields, cancellationToken);
            UpdateStatusesOnOccurence(occurenceResult, statusResults);
        }

        return occurenceResult;
    }

    /// <summary>
    /// Updates the status properties on an occurrence based on the results of status patch operations.
    /// </summary>
    /// <param name="occurence">The occurrence to update.</param>
    /// <param name="statusResults">The list of status patch results.</param>
    private static void UpdateStatusesOnOccurence(Occurence occurence, List<StatusPatchResult> statusResults)
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

    /// <summary>
    /// Updates multiple status fields for a given occurrence.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="id">The ID of the occurrence.</param>
    /// <param name="statusFields">A dictionary of status field names and their values.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of <see cref="StatusPatchResult"/> objects representing the results of each status update.</returns>
    private static async Task<List<StatusPatchResult>> UpdateStatusesAsync(this IdRofusClient client, int? id, Dictionary<string, object> statusFields, CancellationToken cancellationToken)
    {
        var results = new List<StatusPatchResult>();

        if (id is null)
            throw new ArgumentNullException(nameof(id), "Occurrence ID cannot be null for status update.");

        foreach (var prop in statusFields)
        {
            // Use the new extension method for KeyValuePair<string, object>
            var query = prop.ToStatusPatchOption();

            var result = await client.UpdateOccurrenceStatusAsync(id.Value, query, cancellationToken);
            results.Add(result);
        }

        return results;
    }

    /// <summary>
    /// Updates a single status field for a given occurrence.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="id">The ID of the occurrence.</param>
    /// <param name="query">The status patch request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The <see cref="StatusPatchResult"/> of the update operation.</returns>
    public static async Task<StatusPatchResult> UpdateOccurrenceStatusAsync(this IdRofusClient client, int id, StatusPatchRequest query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Occurrences.CombineToRequest(id, "statuses", query.StatusTypeId.ToString());
        var result = await client.PatchAsync<StatusPatchBody>(request, query, cancellationToken);

        return new StatusPatchResult
        {
            Code = result.Code,
            StatusId = result.StatusId,
            PropertyName = query.PropertyName
        };
    }

    /// <summary>
    /// Deletes an occurrence by its ID.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="id">The ID of the occurrence to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public static Task DeleteOccurrenceAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync<Occurence>(dRofusType.Occurrences.CombineToRequest(id), null, cancellationToken);
    }
}
