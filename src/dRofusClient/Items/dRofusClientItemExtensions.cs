using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using dRofusClient.Files;
using dRofusClient.Models;

namespace dRofusClient.Items;

/// <summary>
/// Data transfer object for Item
/// </summary>
public record dRofusItem : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Data transfer object for Item File
/// </summary>
public record dRofusFile : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API
}

/// <summary>
/// Data transfer object for File Upload Response
/// </summary>
public record dRofusFileUploadResponse : dRofusDto
{
    public int Id { get; init; }
    public string? FileName { get; init; }
    // Additional properties from the API
}

/// <summary>
/// Options for uploading a file
/// </summary>
public record dRofusFileUploadOptions
{
    public Stream FileContent { get; init; } = null!;
    public string FileName { get; init; } = string.Empty;
    public string? Description { get; init; }
}

/// <summary>
/// Data transfer object for Item Image
/// </summary>
public record dRofusImage : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API
}

/// <summary>
/// Options for uploading an image
/// </summary>
public record dRofusImageUploadOptions
{
    public Stream ImageContent { get; init; } = null!;
    public string FileName { get; init; } = string.Empty;
    public string? Note { get; init; }
}

/// <summary>
/// Data transfer object for Item Log
/// </summary>
public record dRofusItemLog : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API
}

/// <summary>
/// Data transfer object for Sub Item
/// </summary>
public record dRofusSubItem : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API
}

/// <summary>
/// Extension methods for working with Items in dRofus API
/// </summary>
public static class dRofusClientItemExtensions
{
    /// <summary>
    /// Get list of Items with the specified options
    /// </summary>
    public static Task<List<dRofusItem>> GetItemsAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusItem>(dRofusType.Items.ToRequest(), options, cancellationToken);
    }

    /// <summary>
    /// Create new Item
    /// </summary>
    public static Task<dRofusItem> CreateItemAsync(this IdRofusClient client, dRofusItem item, CancellationToken cancellationToken = default)
    {
        return client.PostAsync<dRofusItem>(dRofusType.Items.ToRequest(), item.ToPostOption(), cancellationToken);
    }

    /// <summary>
    /// Get specified Item by ID
    /// </summary>
    public static Task<dRofusItem> GetItemAsync(this IdRofusClient client, int id, dRofusFieldsOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusItem>(dRofusType.Items.CombineToRequest(id), options, cancellationToken);
    }

    /// <summary>
    /// Update an existing Item
    /// </summary>
    public static Task<dRofusItem> UpdateItemAsync(this IdRofusClient client, dRofusItem item, CancellationToken cancellationToken = default)
    {
        return client.PatchAsync<dRofusItem>(dRofusType.Items.CombineToRequest(item.Id), item.ToPatchOption(), cancellationToken);
    }

    /// <summary>
    /// Get files meta for specified Item
    /// </summary>
    public static Task<List<dRofusFile>> GetItemFilesAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusFile>(dRofusType.Items.CombineToRequest(id, "files"), options, cancellationToken);
    }

    /// <summary>
    /// Upload a new file to a specified Item
    /// </summary>
    public static Task<dRofusFileUploadResponse> UploadItemFileAsync(this IdRofusClient client, int id, dRofusFileUploadOptions options, CancellationToken cancellationToken = default)
    {
        return client.PostFileAsync<dRofusFileUploadResponse>(dRofusType.Items.CombineToRequest(id, "files"), options, cancellationToken);
    }

    /// <summary>
    /// Add link to an existing file for this item
    /// </summary>
    public static Task LinkFileToItemAsync(this IdRofusClient client, int id, int fileId, CancellationToken cancellationToken = default)
    {
        return client.PostAsync<EmptyDto>(dRofusType.Items.CombineToRequest(id, "files", fileId.ToString()), null, cancellationToken);
    }

    /// <summary>
    /// Remove link to a file from this item (does not delete the file)
    /// </summary>
    public static Task UnlinkFileFromItemAsync(this IdRofusClient client, int id, int fileId, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync<EmptyDto>(dRofusType.Items.CombineToRequest(id, "files", fileId.ToString()), null, cancellationToken);
    }

    /// <summary>
    /// Get list of image metas for a specified Item
    /// </summary>
    public static Task<List<dRofusImage>> GetItemImagesAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusImage>(dRofusType.Items.CombineToRequest(id, "images"), options, cancellationToken);
    }

    /// <summary>
    /// Upload a new image to a specified Item
    /// </summary>
    public static Task<dRofusImage> UploadItemImageAsync(this IdRofusClient client, int id, dRofusImageUploadOptions options, CancellationToken cancellationToken = default)
    {
        return client.PostFileAsync<dRofusImage>(dRofusType.Items.CombineToRequest(id, "images"), options, cancellationToken);
    }

    /// <summary>
    /// Get log entries for specified Item
    /// </summary>
    public static Task<dRofusItemLog> GetItemLogsAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusItemLog>(dRofusType.Items.CombineToRequest(id, "logs"), options, cancellationToken);
    }

    /// <summary>
    /// Gets list of sub-items for item, if any
    /// </summary>
    public static Task<dRofusSubItem> GetItemSubItemsAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusSubItem>(dRofusType.Items.CombineToRequest(id, "subitems"), options, cancellationToken);
    }

    /// <summary>
    /// Get log entries for Items
    /// </summary>
    public static Task<List<dRofusItemLog>> GetItemsLogsAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusItemLog>(dRofusType.Items.CombineToRequest("logs"), options, cancellationToken);
    }

    /// <summary>
    /// Get specified Item image
    /// </summary>
    public static Task<byte[]> GetItemImageAsync(this IdRofusClient client, int imageId, CancellationToken cancellationToken = default)
    {
        return client.GetBytesAsync(dRofusType.Items.CombineToRequest("images", imageId.ToString()), cancellationToken);
    }

    /// <summary>
    /// Get specified Item image meta
    /// </summary>
    public static Task<dRofusImage> GetItemImageMetaAsync(this IdRofusClient client, int imageId, dRofusFieldsOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusImage>(dRofusType.Items.CombineToRequest("images", imageId.ToString(), "meta"), options, cancellationToken);
    }

    /// <summary>
    /// Delete specified Item image
    /// </summary>
    public static Task DeleteItemImageAsync(this IdRofusClient client, int imageId, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync<EmptyDto>(dRofusType.Items.CombineToRequest("images", imageId.ToString()), null, cancellationToken);
    }
}