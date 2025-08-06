using dRofusClient.Systems;

namespace dRofusClient.Integration.Tests.Creators;

internal class SystemCreator(IdRofusClient client) : IAsyncDisposable
{
    public int SystemId => 82;
    public SystemInstance? System { get; private set; }
    public async Task InitializeAsync()
    {
        // Since we cannot delete item groups in dRofus, we can try to get an existing one first.
        var existingSystems = await client.GetSystemsAsync(Query.List().Filter(Filter.Eq(SystemInstance.IdField, SystemId)));
        if (existingSystems.Count > 0)
        {
            System = existingSystems[0];
            return;
        }
        
        throw new NotImplementedException("Creating systems is not implemented in dRofusClient yet.");
    }
    public ValueTask DisposeAsync()
    {
        // Its not possible to delete item groups in dRofus, so we don't do anything here.
        return ValueTask.CompletedTask;
    }
}

public class SystemFixture : ClientSetupFixture, IAsyncLifetime
{
    private SystemCreator? _systemCreator;
    private bool _isDisposed;

    public SystemInstance System => _systemCreator?.System ?? throw new InvalidOperationException("System has not been created or initialized.");

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _systemCreator = new SystemCreator(Client);
        await _systemCreator.InitializeAsync();
    }

    public override async Task DisposeAsync()
    {
        if (_isDisposed)
            return;

        try
        {
            if (_systemCreator is null)
                throw new InvalidOperationException("SystemCreator has not been initialized.");

            await _systemCreator.DisposeAsync();
            _isDisposed = true;
        }
        finally
        {
            await base.DisposeAsync();
        }
    }
}