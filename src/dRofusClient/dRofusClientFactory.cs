// ReSharper disable InconsistentNaming

namespace dRofusClient;

[GenerateInterface]
public class dRofusClientFactory : IdRofusClientFactory
{
    readonly HttpClient _httpClient = new();

    public IdRofusClient Create(dRofusConnectionArgs connectionArgs)
    {
        var client = new dRofusClient(_httpClient);
        client.Setup(connectionArgs);
        return client;
    }
}