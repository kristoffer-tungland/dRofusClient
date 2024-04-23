// ReSharper disable InconsistentNaming

namespace dRofusClient;

[Register<IdRofusClientFactory>, GenerateInterface]
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