using dRofusClient.ItemGroups;

namespace dRofusClient.Integration.Tests.Creators;


internal class ItemGroupCreator(IdRofusClient client) : IAsyncDisposable
{
    public CreateItemGroup ItemGroupToCreate { get; } = new()
    {
        Number = "TestGroup",
        Name = "Test Item Group"
    };

    public ItemGroup? ItemGroup { get; private set; }

    public async Task InitializeAsync()
    {
        // Since we cannot delete item groups in dRofus, we can try to get an existing one first.
        var existingItemGroups = await client.GetItemGroupsAsync(Query.List().Filter(Filter.Eq(ItemGroup.NumberField, ItemGroupToCreate.Number)));

        if (existingItemGroups.Count > 0)
        {
            ItemGroup = existingItemGroups[0];
            return;
        }

        ItemGroup = await client.CreateItemGroupAsync(ItemGroupToCreate);

        if (ItemGroup is null)
            throw new InvalidOperationException("Failed to create item group.");
    }

    public ValueTask DisposeAsync()
    {
        // Its not possible to delete item groups in dRofus, so we don't do anything here.
        return ValueTask.CompletedTask;
    }
}

public class ItemGroupFixture : ClientSetupFixture, IAsyncLifetime
{
    private ItemGroupCreator? _itemGroupCreator;
    private bool _isDisposed;

    public ItemGroup ItemGroup => _itemGroupCreator?.ItemGroup ?? throw new InvalidOperationException("ItemGroup has not been created or initialized.");

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _itemGroupCreator = new ItemGroupCreator(Client);
        await _itemGroupCreator.InitializeAsync();
    }

    public override async Task DisposeAsync()
    {
        if (_isDisposed)
            return;

        try
        {
            if (_itemGroupCreator is null)
                throw new InvalidOperationException("ItemGroupCreator has not been initialized.");

            await _itemGroupCreator.DisposeAsync();
            _isDisposed = true;
        }
        finally
        {
            await base.DisposeAsync();
        }
    }
}