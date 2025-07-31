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
}
