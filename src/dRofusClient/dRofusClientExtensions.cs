namespace dRofusClient;

public static class dRofusClientExtensions
{
    public static Task<TResult> GetAsync<TResult>(this IdRofusClient client,
        string route,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
    ) where TResult : dRofusDto, new()
    {
        return client.SendAsync<TResult>(HttpMethod.Get, route, options, cancellationToken);
    }

    public static Task<TResult> PostAsync<TResult>(this IdRofusClient client,
        string route,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
    ) where TResult : dRofusDto, new()
    {
        return client.SendAsync<TResult>(HttpMethod.Post, route, options, cancellationToken);
    }

    public static Task<TResult> PatchAsync<TResult>(this IdRofusClient client,
        string route,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
    ) where TResult : dRofusDto, new()
    {
        return client.SendAsync<TResult>(new HttpMethod("PATCH"), route, options, cancellationToken);
    }

    public static Task<TResult> DeleteAsync<TResult>(this IdRofusClient client,
        string route,
        dRofusOptionsBase? options = default,
        CancellationToken cancellationToken = default
    ) where TResult : dRofusDto, new()
    {
        return client.SendAsync<TResult>(HttpMethod.Delete, route, options, cancellationToken);
    }
}