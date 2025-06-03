using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using dRofusClient.PropertyMeta;
using dRofusClient.Items;

namespace dRofusClient.Products;

/// <summary>
/// Data transfer object for Product
/// </summary>
public record dRofusProduct : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Extension methods for working with Products in dRofus API
/// </summary>
public static class dRofusClientProductExtensions
{
    /// <summary>
    /// Get list of Products with the specified options
    /// </summary>
    public static Task<List<dRofusProduct>> GetProductsAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusProduct>(dRofusType.Products.ToRequest(), options, cancellationToken);
    }

    /// <summary>
    /// Get specified Product by ID
    /// </summary>
    public static Task<dRofusProduct> GetProductAsync(this IdRofusClient client, int id, dRofusFieldsOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusProduct>(dRofusType.Products.CombineToRequest(id), options, cancellationToken);
    }

    /// <summary>
    /// Get files meta for specified Product
    /// </summary>
    public static Task<List<Items.dRofusFile>> GetProductFilesAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<Items.dRofusFile>(dRofusType.Products.CombineToRequest(id, "files"), options, cancellationToken);
    }

    /// <summary>
    /// Upload a new file to a specified Product
    /// </summary>
    public static Task<Items.dRofusFileUploadResponse> UploadProductFileAsync(this IdRofusClient client, int id, Items.dRofusFileUploadOptions options, CancellationToken cancellationToken = default)
    {
        return client.PostFileAsync<Items.dRofusFileUploadResponse>(dRofusType.Products.CombineToRequest(id, "files"), options, cancellationToken);
    }

    /// <summary>
    /// Get meta information about available properties for Products
    /// </summary>
    public static Task<List<dRofusPropertyMeta>> GetProductPropertyMetaAsync(this IdRofusClient client, 
        dRofusPropertyMetaOptions? options = default, 
        CancellationToken cancellationToken = default)
    {
        return client.OptionsListAsync<dRofusPropertyMeta>(dRofusType.Products.ToRequest(), options, cancellationToken);
    }
}