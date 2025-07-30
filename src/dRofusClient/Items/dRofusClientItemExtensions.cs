namespace dRofusClient.Items;

/// <summary>
/// Extension methods for working with Item resources using the IdRofusClient.
/// </summary>
public static class dRofusClientItemExtensions
{
    /// <summary>
    /// Retrieves a list of items matching the specified query.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="query">The query parameters for filtering and paging.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of <see cref="Item"/> objects.</returns>
    public static Task<List<Item>> GetItemsAsync(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<Item>(dRofusType.Items.ToRequest(), query, cancellationToken);
    }

    /// <summary>
    /// Creates a new item.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="item">The item to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created <see cref="Item"/> object.</returns>
    public static Task<Item> CreateItemAsync(this IdRofusClient client, CreateItem itemToCreate, CancellationToken cancellationToken = default)
    {
        return client.PostAsync<Item>(dRofusType.Items.ToRequest(), itemToCreate.ToPostRequest(), cancellationToken);
    }

    /// <summary>
    /// Retrieves a single item by its ID.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="id">The ID of the item.</param>
    /// <param name="query">Optional query parameters for field selection.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The <see cref="Item"/> object with the specified ID.</returns>
    public static async Task<Item> GetItemAsync(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
    {
        var item = await client.GetAsync<Item>(dRofusType.Items.CombineToRequest(id), query, cancellationToken);
        return item;
    }

    /// <summary>
    /// Updates an existing item.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="item">The item to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated <see cref="Item"/> object.</returns>
    public static async Task<Item> UpdateItemAsync(this IdRofusClient client, Item item, CancellationToken cancellationToken = default)
    {
        item = item with
        {
            ClassificationNumber = null,
            Number = null,
            PriceDate = null,
            Responsibility = null,
            CreatedBy = null,
            NumberName = null,
            Created = null,
        };


        var patchOptions = item.ToPatchRequest();
        Item? itemResult = null;
        if (patchOptions.Body is not null && patchOptions.Body.Equals("{}") == false)
            itemResult = await client.PatchAsync<Item>(dRofusType.Items.CombineToRequest(item.Id), patchOptions, cancellationToken);
        itemResult ??= item with { Id = item.Id, AdditionalProperties = item.AdditionalProperties };
        return itemResult;
    }

    /// <summary>
    /// Deletes an item by its ID.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="id">The ID of the item to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    [Obsolete("It's not possible to delete items in dRofus, so this method is not implemented.")]
    public static Task DeleteItemAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("It's not possible to delete items in dRofus, so this method is not implemented.");
    }
}
