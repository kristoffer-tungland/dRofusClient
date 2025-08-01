namespace dRofusClient.SystemComponents;

/// <summary>
/// Extension methods for system component endpoints.
/// </summary>
public static class dRofusClientSystemComponentExtensions
{
    /// <summary>
    /// Retrieves a list of system components.
    /// </summary>
    public static Task<List<SystemComponent>> GetSystemComponentsAsync(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
    {
        return client.GetListAsync<SystemComponent>(dRofusType.SystemComponents.ToRequest(), query, cancellationToken);
    }

    /// <summary>
    /// Retrieves the specified system component by id.
    /// </summary>
    public static Task<SystemComponent> GetSystemComponentAsync(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
    {
        return client.GetAsync<SystemComponent>(dRofusType.SystemComponents.CombineToRequest(id), query, cancellationToken);
    }

    /// <summary>
    /// Retrieves components within the specified system component.
    /// </summary>
    public static Task<List<Component>> GetSystemComponentComponentsAsync(this IdRofusClient client, int parentId, ListQuery query, CancellationToken cancellationToken = default)
    {
        var request = dRofusType.SystemComponents.CombineToRequest(parentId, "components");
        return client.GetListAsync<Component>(request, query, cancellationToken);
    }
}
