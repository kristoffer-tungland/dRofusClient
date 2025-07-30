using dRofusClient.Options;
using dRofusClient.Revit.Utils;

namespace dRofusClient.ItemGroups
{
    /// <summary>
    /// Synchronous extension methods for working with Item Group resources using the IdRofusClient.
    /// </summary>
    public static class dRofusClientRevitItemGroupExtensions
    {
        /// <summary>
        /// Retrieves a list of item groups matching the specified query.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="query">The query parameters for filtering and paging.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="ItemGroup"/> objects.</returns>
        public static List<ItemGroup> GetItemGroups(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetItemGroupsAsync(query, cancellationToken)
            );
        }

        /// <summary>
        /// Creates a new item group.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="itemGroupToCreate">The item group to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created <see cref="ItemGroup"/> object.</returns>
        public static ItemGroup CreateItemGroup(this IdRofusClient client, CreateItemGroup itemGroupToCreate, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.CreateItemGroupAsync(itemGroupToCreate, cancellationToken)
            );
        }

        /// <summary>
        /// Retrieves a single item group by its ID.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="id">The ID of the item group.</param>
        /// <param name="query">Optional query parameters for field selection.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="ItemGroup"/> object with the specified ID.</returns>
        public static ItemGroup GetItemGroup(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetItemGroupAsync(id, query, cancellationToken)
            );
        }

        /// <summary>
        /// Updates an existing item group.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="itemGroup">The item group to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated <see cref="ItemGroup"/> object.</returns>
        public static ItemGroup UpdateItemGroup(this IdRofusClient client, ItemGroup itemGroup, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.UpdateItemGroupAsync(itemGroup, cancellationToken)
            );
        }

        /// <summary>
        /// Deletes an item group by its ID. Not supported in dRofus.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="id">The ID of the item group.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        [Obsolete("Deleting item groups is not supported in dRofus.")]
        public static void DeleteItemGroup(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Deleting item groups is not supported in dRofus.");
        }
    }
}
