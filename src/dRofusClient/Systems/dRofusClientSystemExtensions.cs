using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using dRofusClient.Files;
using dRofusClient.Occurrences;
using dRofusClient.Models;

namespace dRofusClient.Systems;

/// <summary>
/// Data transfer object for System
/// </summary>
public record dRofusSystem : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Data transfer object for System Log
/// </summary>
public record dRofusSystemLog : dRofusDto
{
    public int Id { get; init; }
    // Additional properties from the API can be added here
}

/// <summary>
/// Extension methods for working with Systems in dRofus API
/// </summary>
public static class dRofusClientSystemExtensions
{
    /// <summary>
    /// Get list of Systems with the specified options
    /// </summary>
    public static Task<List<dRofusSystem>> GetSystemsAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusSystem>(dRofusType.Systems.ToRequest(), options, cancellationToken);
    }

    /// <summary>
    /// Get specified System by ID
    /// </summary>
    public static Task<dRofusSystem> GetSystemAsync(this IdRofusClient client, int id, dRofusFieldsOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusSystem>(dRofusType.Systems.CombineToRequest(id), options, cancellationToken);
    }

    /// <summary>
    /// Update an existing System
    /// </summary>
    public static Task<dRofusSystem> UpdateSystemAsync(this IdRofusClient client, dRofusSystem system, CancellationToken cancellationToken = default)
    {
        return client.PatchAsync<dRofusSystem>(dRofusType.Systems.CombineToRequest(system.Id), system.ToPatchOption(), cancellationToken);
    }

    /// <summary>
    /// Delete a system by ID
    /// </summary>
    public static Task DeleteSystemAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync<EmptyDto>(dRofusType.Systems.CombineToRequest(id), null, cancellationToken);
    }

    /// <summary>
    /// Get components for a specified System
    /// </summary>
    public static Task<dRofusOccurence> GetSystemComponentsAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<dRofusOccurence>(dRofusType.Systems.CombineToRequest(id, "components"), options, cancellationToken);
    }

    /// <summary>
    /// Get files meta for specified System
    /// </summary>
    public static Task<List<dRofusFile>> GetSystemFilesAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusFile>(dRofusType.Systems.CombineToRequest(id, "files"), options, cancellationToken);
    }

    /// <summary>
    /// Get logs for a specified System
    /// </summary>
    public static Task<List<dRofusSystemLog>> GetSystemLogsAsync(this IdRofusClient client, int id, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusSystemLog>(dRofusType.Systems.CombineToRequest(id, "logs"), options, cancellationToken);
    }

    /// <summary>
    /// Get logs for all Systems
    /// </summary>
    public static Task<List<dRofusSystemLog>> GetSystemsLogsAsync(this IdRofusClient client, dRofusListOptions options, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<dRofusSystemLog>(dRofusType.Systems.CombineToRequest("logs"), options, cancellationToken);
    }
}