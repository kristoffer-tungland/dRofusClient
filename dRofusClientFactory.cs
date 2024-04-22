// ReSharper disable InconsistentNaming
namespace dRofusClient;

[Register<IdRofusClientFactory>, GenerateInterface]
public class dRofusClientFactory : IdRofusClientFactory
{
    public async Task<IdRofusClient> Create(dRofusConnectionArgs connectionArgs, CancellationToken cancellationToken = default)
    {
        var client = new dRofusClient();
        await client.Login(connectionArgs, cancellationToken);
        return client;
    }
}