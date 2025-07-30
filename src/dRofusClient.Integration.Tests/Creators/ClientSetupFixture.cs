using Microsoft.Extensions.Configuration;

namespace dRofusClient.Integration.Tests.Creators;

public class ClientSetupFixture : IAsyncLifetime
{
    private bool _isDisposed;

    public IdRofusClient Client { get; }

    public ClientSetupFixture()
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<ClientSetupFixture>()
            .Build();

        var connectionArgs = dRofusConnectionArgs.Create(
            baseUrl: config["dRofus:BaseUrl"],
            database: config["dRofus:Database"],
            projectId: config["dRofus:ProjectId"],
            username: config["dRofus:Username"],
            password: config["dRofus:Password"]);

        Client = new dRofusClientFactory().Create(connectionArgs);
    }

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task DisposeAsync()
    {
        if (_isDisposed)
            return Task.CompletedTask;

        Client?.Dispose();
        _isDisposed = true;
        return Task.CompletedTask;
    }
}