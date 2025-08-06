namespace dRofusClient.Systems;

/// <summary>
/// Extension methods for system related endpoints.
/// </summary>
public static class dRofusClientSystemExtensions
{
    /// <summary>
    /// Retrieves logs for all systems.
    /// </summary>
    public static Task<List<ApiLogs.SystemLog>> GetSystemLogsAsync(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Systems.CombineToRequest("logs");
        return client.GetListAsync<ApiLogs.SystemLog>(request, query, cancellationToken);
    }

    /// <summary>
    /// Retrieves logs for a specific system.
    /// </summary>
    public static Task<List<ApiLogs.SystemLog>> GetSystemLogsAsync(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Systems.CombineToRequest(id, "logs");
        return client.GetListAsync<ApiLogs.SystemLog>(request, query, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of systems.
    /// </summary>
    public static Task<List<SystemInstance>> GetSystemsAsync(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<SystemInstance>(dRofusType.Systems.ToRequest(), query, cancellationToken);
    }

    /// <summary>
    /// Retrieves the specified system by id.
    /// </summary>
    public static Task<SystemInstance> GetSystemAsync(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<SystemInstance>(dRofusType.Systems.CombineToRequest(id), query, cancellationToken);
    }

    /// <summary>
    /// Updates an existing system.
    /// </summary>
    public static async Task<SystemInstance> UpdateSystemAsync(this IdRofusClient client, SystemInstance system, CancellationToken cancellationToken = default)
    {
        system = system.ClearReadOnlyFields();
        var patchOptions = system.ToPatchRequest();
        SystemInstance? result = null;
        if (patchOptions.Body is not null && patchOptions.Body != "{}")
            result = await client.PatchAsync<SystemInstance>(dRofusType.Systems.CombineToRequest(system.Id), patchOptions, cancellationToken);
        result ??= system with { Id = system.Id };
        return result;
    }

    /// <summary>
    /// Deletes the specified system.
    /// </summary>
    public static Task DeleteSystemAsync(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
    {
        return client.DeleteAsync<SystemInstance>(dRofusType.Systems.CombineToRequest(id), null, cancellationToken);
    }

    /// <summary>
    /// Retrieves components that belong to a system.
    /// </summary>
    public static Task<List<SystemComponents.Component>> GetSystemComponentsAsync(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Systems.CombineToRequest(id, "components");
        return client.GetListAsync<SystemComponents.Component>(request, query, cancellationToken);
    }

    /// <summary>
    /// Retrieves files associated with a system.
    /// </summary>
    public static Task<List<Files.FileDetails>> GetSystemFilesAsync(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.Systems.CombineToRequest(id, "files");
        return client.GetListAsync<Files.FileDetails>(request, query, cancellationToken);
    }
}
