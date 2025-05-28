// ReSharper disable InconsistentNaming

using System.Collections.Concurrent;

namespace dRofusClient;

[GenerateInterface]
public class dRofusClientFactory() : IdRofusClientFactory
{
    private ILoginPromptHandler _defaultLoginPromtHandler = new NonePromptHandler();

    // Make the cache static so it is shared across all instances
    private static readonly ConcurrentDictionary<(string baseAddress, string database, string projectId), HttpClient> _httpClientCache
        = new();

    public IdRofusClient Create(dRofusConnectionArgs connectionArgs, ILoginPromptHandler? loginPromptHandler = default)
    {
        loginPromptHandler ??= _defaultLoginPromtHandler;
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

    public void SetDefaultLoginPromptHandler(ILoginPromptHandler loginPromptHandler)
    {
        _defaultLoginPromtHandler = loginPromptHandler ?? throw new ArgumentNullException(nameof(loginPromptHandler), "Login prompt handler cannot be null.");
    }
}

public static class dRofusClientFactoryExtensions
{
    public static dRofusClientFactory ConfigureLoginPromptHandler(this dRofusClientFactory factory, Func<dRofusClientFactory, ILoginPromptHandler> setupAction)
    {
        if (setupAction == null)
            throw new ArgumentNullException(nameof(setupAction), "Setup action cannot be null.");
        var handler = setupAction(factory);
        factory.SetDefaultLoginPromptHandler(handler);
        return factory;
    }
}