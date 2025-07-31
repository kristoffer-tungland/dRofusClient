using dRofusClient.ApiLogs;
using dRofusClient.Files;
using System.IO;

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
        item = item.ClearReadOnlyFields();

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

    /// <summary>
    /// Retrieves log entries for items.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="query">Query parameters for filtering and paging.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task<List<ItemLog>> GetItemLogsAsync(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Items.CombineToRequest("logs");
        return client.GetListAsync<ItemLog>(request, query, cancellationToken);
    }

    /// <summary>
    /// Retrieves log entries for the specified item.
    /// </summary>
    /// <param name="client">The dRofus client instance.</param>
    /// <param name="itemId">ID of the item.</param>
    /// <param name="query">Query parameters for filtering and paging.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task<List<ItemLog>> GetItemLogsAsync(this IdRofusClient client, int itemId, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Items.CombineToRequest(itemId, "logs");
        return client.GetListAsync<ItemLog>(request, query, cancellationToken);
    }

    /// <summary>
    /// Retrieves file metadata for the specified item.
    /// </summary>
    public static Task<List<Files.FileDetails>> GetItemFilesAsync(this IdRofusClient client, int itemId, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Items.CombineToRequest(itemId, "files");
        return client.GetListAsync<Files.FileDetails>(request, query, cancellationToken);
    }

    /// <summary>
    /// Uploads a new file to the specified item.
    /// </summary>
    public static async Task<Files.FileUploadResponse> UploadItemFileAsync(this IdRofusClient client, int itemId, Stream fileStream, string fileName, string? description = null, CancellationToken cancellationToken = default)
    {
        var (db, pr) = client.GetDatabaseAndProjectId();
        var url = $"/api/{db}/{pr}/" + dRofusType.Items.CombineToRequest(itemId, "files");

        using var form = new MultipartFormDataContent();
        form.Add(new StreamContent(fileStream), "file", fileName);
        if (!string.IsNullOrEmpty(description))
            form.Add(new StringContent(description), "description");

        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = form };
        var response = await client.HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Files.FileUploadResponse>(cancellationToken) ?? new Files.FileUploadResponse();
    }

    /// <summary>
    /// Adds a link to an existing file for the specified item.
    /// </summary>
    public static async Task AddItemFileLinkAsync(this IdRofusClient client, int itemId, int fileId, CancellationToken cancellationToken = default)
    {
        var (db, pr) = client.GetDatabaseAndProjectId();
        var url = $"/api/{db}/{pr}/" + dRofusType.Items.CombineToRequest(itemId, "files", fileId.ToString());
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        var response = await client.HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Removes a file link from the specified item.
    /// </summary>
    public static async Task RemoveItemFileLinkAsync(this IdRofusClient client, int itemId, int fileId, CancellationToken cancellationToken = default)
    {
        var (db, pr) = client.GetDatabaseAndProjectId();
        var url = $"/api/{db}/{pr}/" + dRofusType.Items.CombineToRequest(itemId, "files", fileId.ToString());
        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        var response = await client.HttpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Retrieves image metadata for the specified item.
    /// </summary>
    public static Task<List<Files.Image>> GetItemImagesAsync(this IdRofusClient client, int itemId, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Items.CombineToRequest(itemId, "images");
        return client.GetListAsync<Files.Image>(request, query, cancellationToken);
    }
}
