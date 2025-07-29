using Microsoft.Extensions.Configuration;

namespace dRofusClient.Integration.Tests;

public class SetupFixture : IDisposable
{
    public IdRofusClient Client { get; }

    public SetupFixture()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<SetupFixture>()
            .Build();

        var connectionArgs = dRofusConnectionArgs.Create(
            baseUrl: config["dRofus:BaseUrl"],
            database: config["dRofus:Database"],
            projectId: config["dRofus:ProjectId"],
            username: config["dRofus:Username"],
            password: config["dRofus:Password"]);

        Client = new dRofusClientFactory().Create(connectionArgs);
    }

    public void Dispose()
    {
        Client?.Dispose();
    }
}
