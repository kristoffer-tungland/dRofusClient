using dRofusClient.SystemComponents;
using dRofusClient.Systems;

namespace dRofusClient.Integration.Tests.Creators;

internal class SystemComponentCreator(IdRofusClient client) : IAsyncDisposable
{
    public int ComponentId => 957;
    public SystemComponent? Component { get; private set; }
    public async Task InitializeAsync()
    {
        // Since we cannot delete system components in dRofus, we can try to get an existing one first.
        var existingComponents = await client.GetSystemComponentsAsync(Query.List().Filter(Filter.Eq(SystemComponent.IdField, ComponentId)));
        if (existingComponents.Count > 0)
        {
            Component = existingComponents[0];
            return;
        }
        
        throw new NotImplementedException("Creating system components is not implemented in dRofusClient yet.");
    }
    public ValueTask DisposeAsync()
    {
        // Its not possible to delete system components in dRofus, so we don't do anything here.
        return ValueTask.CompletedTask;
    }
}

public class SystemComponentFixture : ClientSetupFixture, IAsyncLifetime
{
    private SystemComponentCreator? _systemComponentCreator;
    private bool _isDisposed;
    public SystemComponent SystemComponent => _systemComponentCreator?.Component ?? throw new InvalidOperationException("SystemComponent has not been created or initialized.");
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _systemComponentCreator = new SystemComponentCreator(Client);
        await _systemComponentCreator.InitializeAsync();
    }
    public override async Task DisposeAsync()
    {
        if (_isDisposed)
            return;
        try
        {
            if (_systemComponentCreator is null)
                throw new InvalidOperationException("SystemComponentCreator has not been initialized.");
            await _systemComponentCreator.DisposeAsync();
            _isDisposed = true;
        }
        finally
        {
            await base.DisposeAsync();
        }
    }
}
