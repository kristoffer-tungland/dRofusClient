// ReSharper disable InconsistentNaming

using System.Collections.Concurrent;

namespace dRofusClient;

[GenerateInterface]
public class dRofusClientFactory : IdRofusClientFactory
{
    // Make the cache static so it is shared across all instances
    private static readonly ConcurrentDictionary<(string baseAddress, string database, string projectId), HttpClient> _httpClientCache
        = new();

    public IdRofusClient Create(dRofusConnectionArgs connectionArgs, ILoginPromptHandler? loginPromptHandler = default)
    {
        loginPromptHandler ??= new NonePromptHandler();
        var baseAddress = connectionArgs.BaseUrl?.TrimEnd('/');
        var database = connectionArgs.Database;
        var projectId = connectionArgs.ProjectId;

        var key = (baseAddress, database, projectId);

        var httpClient = _httpClientCache.GetOrAdd(key, _ =>
        {
            var client = new HttpClient();
            if (!string.IsNullOrEmpty(baseAddress))
                client.BaseAddress = new Uri(baseAddress);
            return client;
        });

        // Pass the loginPromptHandler to dRofusClient
        var clientInstance = new dRofusClient(httpClient, loginPromptHandler);
        clientInstance.Setup(connectionArgs);
        return clientInstance;
    }

    public IdRofusClient Create(ILoginPromptHandler? loginPromptHandler = default)
    {
        loginPromptHandler ??= new NonePromptHandler();
        var httpClient = new HttpClient();
        var client = new dRofusClient(httpClient, loginPromptHandler);
        return client;
    }
}