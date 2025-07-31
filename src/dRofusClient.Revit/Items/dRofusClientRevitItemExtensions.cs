using dRofusClient.Options;
using dRofusClient.Revit.Utils;

namespace dRofusClient.Items
{
    /// <summary>
    /// Synchronous extension methods for working with Item resources using the IdRofusClient.
    /// </summary>
    public static class dRofusClientRevitItemExtensions
    {
        /// <summary>
        /// Retrieves a list of items matching the specified query.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="query">The query parameters for filtering and paging.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="Item"/> objects.</returns>
        public static List<Item> GetItems(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetItemsAsync(query, cancellationToken)
            );
        }

        /// <summary>
        /// Creates a new item.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="itemToCreate">The item to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created <see cref="Item"/> object.</returns>
        public static Item CreateItem(this IdRofusClient client, CreateItem itemToCreate, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.CreateItemAsync(itemToCreate, cancellationToken)
            );
        }

        /// <summary>
        /// Retrieves a single item by its ID.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="id">The ID of the item.</param>
        /// <param name="query">Optional query parameters for field selection.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="Item"/> object with the specified ID.</returns>
        public static Item GetItem(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetItemAsync(id, query, cancellationToken)
            );
        }

        /// <summary>
        /// Updates an existing item.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="item">The item to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated <see cref="Item"/> object.</returns>
        public static Item UpdateItem(this IdRofusClient client, Item item, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.UpdateItemAsync(item, cancellationToken)
            );
        }

        /// <summary>
        /// Deletes an item by its ID. Not implemented in dRofus.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="id">The ID of the item to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        [Obsolete("It's not possible to delete items in dRofus, so this method is not implemented.")]
        public static void DeleteItem(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException("It's not possible to delete items in dRofus, so this method is not implemented.");
        }
    }
}
