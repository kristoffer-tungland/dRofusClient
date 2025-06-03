using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using dRofusClient.PropertyMeta;
using dRofusClient.Models;

namespace dRofusClient.Files;

/// <summary>
/// Data transfer object for File
/// </summary>
public record dRofusFile : dRofusDto
{
    public int Id { get; init; }
    public string? FileName { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Extension methods for working with Files in dRofus API
/// </summary>
public static class dRofusClientFileExtensions
{
    /// <summary>
    /// Get list of file meta information
    /// </summary>
    public static Task<List<dRofusFile>> GetFilesAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusFile>(dRofusType.Files.ToRequest(), options, cancellationToken);
    }

    /// <summary>
    /// Get file meta information by specified id
    /// </summary>
    public static Task<dRofusFile> GetFileAsync(this IdRofusClient client, int id, dRofusFieldsOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusFile>(dRofusType.Files.CombineToRequest(id), options, cancellationToken);
    }

    /// <summary>
    /// Completely removes a file
    /// </summary>
    public static Task DeleteFileAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync<EmptyDto>(dRofusType.Files.CombineToRequest(id), null, cancellationToken);
    }

    /// <summary>
    /// Get meta information about available properties for Files
    /// </summary>
    public static Task<List<dRofusPropertyMeta>> GetFilePropertyMetaAsync(this IdRofusClient client, 
        dRofusPropertyMetaOptions? options = default, 
        CancellationToken cancellationToken = default)
    {
        return client.OptionsListAsync<dRofusPropertyMeta>(dRofusType.Files.ToRequest(), options, cancellationToken);
    }

    /// <summary>
    /// Download the file content
    /// </summary>
    public static Task<Stream> DownloadFileAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        return client.GetStreamAsync(dRofusType.Files.CombineToRequest(id, "download"), cancellationToken);
    }
}