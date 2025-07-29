namespace dRofusClient;

public static class dRofusClientListExtensions
{
    public static Task<List<TResult>> GetListAsync<TResult>(this IdRofusClient client,
        string route,
        RequestBase? options = default,
        CancellationToken cancellationToken = default
    ) where TResult : dRofusDto
    {
        return client.SendListAsync<TResult>(HttpMethod.Get, route, options, cancellationToken);
    }

    public static Task<List<TResult>> OptionsListAsync<TResult>(this IdRofusClient client,
        string route,
        RequestBase? options = default,
        CancellationToken cancellationToken = default
    ) where TResult : dRofusDto
    {
        return client.SendListAsync<TResult>(HttpMethod.Options, route, options, cancellationToken);
    }
}