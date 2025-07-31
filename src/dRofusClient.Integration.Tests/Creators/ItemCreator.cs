using dRofusClient.ItemGroups;
using dRofusClient.Items;

namespace dRofusClient.Integration.Tests.Creators;

internal class ItemCreator(IdRofusClient client) : IAsyncDisposable
{
    public string ItemName => "Test Item";
    public Item? Item { get; private set; }

    public async Task InitializeAsync(ItemGroup itemGroup)
    {
        // Since we cannot delete items in dRofus, we can try to get an existing one first.
        var existingItems = await client.GetItemsAsync(Query.List().Filter(Filter.Eq(Item.NameField, ItemName)));

        if (existingItems.Count > 0)
        {
            Item = existingItems[0];
            return;
        }

        Item = await client.CreateItemAsync(CreateItem.With(itemGroup, ItemName));

        if (Item is null)
        {
            throw new InvalidOperationException("Failed to create item.");
        }
    }

    public ValueTask DisposeAsync()
    {
        // Its not possible to delete item groups in dRofus, so we don't do anything here.
        return ValueTask.CompletedTask;
    }
}

public class ItemFixture : ItemGroupFixture
{
    private ItemCreator? _itemCreator;
    private bool _isDisposed;

    public Item Item => _itemCreator?.Item ?? throw new InvalidOperationException("Item has not been created or initialized.");

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        _itemCreator = new ItemCreator(Client);
        await _itemCreator.InitializeAsync(ItemGroup);
    }

    public override async Task DisposeAsync()
    {
        if (_isDisposed)
            return;

        try
        {
            if (_itemCreator is null)
                throw new InvalidOperationException("ItemCreator has not been initialized.");

            await _itemCreator.DisposeAsync();
            _isDisposed = true;
        }
        finally
        {
            await base.DisposeAsync();
        }
    }
}
