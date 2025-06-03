using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using dRofusClient.PropertyMeta;

namespace dRofusClient.ItemGroups;

/// <summary>
/// Data transfer object for Item Group
/// </summary>
public record dRofusItemGroup : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Extension methods for working with Item Groups in dRofus API
/// </summary>
public static class dRofusClientItemGroupExtensions
{
    /// <summary>
    /// Get list of Item Groups with the specified options
    /// </summary>
    public static Task<List<dRofusItemGroup>> GetItemGroupsAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusItemGroup>(dRofusType.ItemGroups.ToRequest(), options, cancellationToken);
    }

    /// <summary>
    /// Create new Item Group
    /// </summary>
    public static Task<dRofusItemGroup> CreateItemGroupAsync(this IdRofusClient client, dRofusItemGroup itemGroup, CancellationToken cancellationToken = default)
    {
        return client.PostAsync<dRofusItemGroup>(dRofusType.ItemGroups.ToRequest(), itemGroup.ToPostOption(), cancellationToken);
    }

    /// <summary>
    /// Get specified Item Group by ID
    /// </summary>
    public static Task<dRofusItemGroup> GetItemGroupAsync(this IdRofusClient client, int id, dRofusFieldsOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusItemGroup>(dRofusType.ItemGroups.CombineToRequest(id), options, cancellationToken);
    }

    /// <summary>
    /// Update an existing Item Group
    /// </summary>
    public static Task<dRofusItemGroup> UpdateItemGroupAsync(this IdRofusClient client, dRofusItemGroup itemGroup, CancellationToken cancellationToken = default)
    {
        return client.PatchAsync<dRofusItemGroup>(dRofusType.ItemGroups.CombineToRequest(itemGroup.Id), itemGroup.ToPatchOption(), cancellationToken);
    }

    /// <summary>
    /// Get meta information about available properties for Item Groups
    /// </summary>
    public static Task<List<dRofusPropertyMeta>> GetItemGroupPropertyMetaAsync(this IdRofusClient client, 
        dRofusPropertyMetaOptions? options = default, 
        CancellationToken cancellationToken = default)
    {
        return client.OptionsListAsync<dRofusPropertyMeta>(dRofusType.ItemGroups.ToRequest(), options, cancellationToken);
    }
}