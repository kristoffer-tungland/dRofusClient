namespace dRofusClient.ItemGroups;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Extension methods for working with Item Group resources using the IdRofusClient.
/// </summary>
public static class dRofusClientItemGroupExtensions
{
    /// <summary>
    /// Retrieves a list of item groups matching the specified query.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="query">The query parameters for filtering and paging.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of <see cref="ItemGroup"/> objects.</returns>
    public static Task<List<ItemGroup>> GetItemGroupsAsync(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<ItemGroup>(dRofusType.ItemGroups.ToRequest(), query, cancellationToken);
    }

    /// <summary>
    /// Creates a new item group.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="itemGroupToCreate">The item group to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created <see cref="ItemGroup"/> object.</returns>
    public static Task<ItemGroup> CreateItemGroupAsync(this IdRofusClient client, CreateItemGroup itemGroupToCreate, CancellationToken cancellationToken = default)
    {
        var existingItemGroup = client.GetItemGroupsAsync(Query.List().Filter(Filter.Eq(ItemGroup.NumberField, itemGroupToCreate.Number)), cancellationToken);

        if (existingItemGroup.Result.Count > 0)
        {
            throw new InvalidOperationException($"Item group with number '{itemGroupToCreate.Number}' already exists.");
        }

        return client.PostAsync<ItemGroup>(dRofusType.ItemGroups.ToRequest(), itemGroupToCreate.ToPostRequest(), cancellationToken);
    }

    /// <summary>
    /// Retrieves a single item group by its ID.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="id">The ID of the item group.</param>
    /// <param name="query">Optional query parameters for field selection.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The <see cref="ItemGroup"/> object with the specified ID.</returns>
    public static Task<ItemGroup> GetItemGroupAsync(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<ItemGroup>(dRofusType.ItemGroups.CombineToRequest(id), query, cancellationToken);
    }

    /// <summary>
    /// Updates an existing item group.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="itemGroup">The item group to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated <see cref="ItemGroup"/> object.</returns>
    public static async Task<ItemGroup> UpdateItemGroupAsync(this IdRofusClient client, ItemGroup itemGroup, CancellationToken cancellationToken = default)
    {
        itemGroup = itemGroup with
        {
            FullNo = null,
            ArticleLevelDepth = null,
        };

        var patchOptions = itemGroup.ToPatchRequest();
        ItemGroup? result = null;
        if (patchOptions.Body is not null && patchOptions.Body.Equals("{}") == false)
            result = await client.PatchAsync<ItemGroup>(dRofusType.ItemGroups.CombineToRequest(itemGroup.Id), patchOptions, cancellationToken);
        result ??= itemGroup with { Id = itemGroup.Id };
        return result;
    }

    /// <summary>
    /// Deletes an item group by its ID.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="id">The ID of the item group.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    [Obsolete("Deleting item groups is not supported in dRofus.")]
    public static Task DeleteItemGroupAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Deleting item groups is not supported in dRofus.");
    }
}