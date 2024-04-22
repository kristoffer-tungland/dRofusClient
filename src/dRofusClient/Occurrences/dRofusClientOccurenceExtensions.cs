namespace dRofusClient.Occurrences;

public static class dRofusClientOccurenceExtensions
{
    public static Task<List<dRofusOccurence>> GetOccurrencesAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.SendListAsync<dRofusOccurence>(HttpMethod.Get, dRofusType.Occurrences, options, cancellationToken);
    }
}