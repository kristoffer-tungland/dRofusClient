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

    public static Task<dRofusOccurence> UpdateOccurrenceAsync(this IdRofusClient client, dRofusOccurence occurence, CancellationToken cancellationToken = default)
    {
        return client.PatchAsync<dRofusOccurence>(dRofusType.Occurrences.CombineToRequest(occurence.Id), occurence.ToPatchOption(), cancellationToken);
    }

    public static Task DeleteOccurrenceAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync<dRofusOccurence>(dRofusType.Occurrences.CombineToRequest(id), null, cancellationToken);
    }
}
