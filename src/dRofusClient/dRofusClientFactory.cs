// ReSharper disable InconsistentNaming

namespace dRofusClient;

[GenerateInterface]
public class dRofusClientFactory : IdRofusClientFactory
{
    public IdRofusClient Create(dRofusConnectionArgs connectionArgs)
    {
        var httpClient = new HttpClient();
        var client = new dRofusClient(httpClient);
        client.Setup(connectionArgs);
        return client;
    }

    public IdRofusClient Create()
    {
        var httpClient = new HttpClient();
        var client = new dRofusClient(httpClient);
        return client;
    }
}