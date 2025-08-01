using dRofusClient.Options;
using dRofusClient.Revit.Utils;

namespace dRofusClient.SystemComponents
{
    /// <summary>
    /// Synchronous extension methods for system component endpoints.
    /// </summary>
    public static class dRofusClientRevitSystemComponentExtensions
    {
        /// <summary>
        /// Retrieves a list of system components.
        /// </summary>
        public static List<SystemComponent> GetSystemComponents(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetSystemComponentsAsync(query, cancellationToken));
        }

        /// <summary>
        /// Retrieves a specific system component.
        /// </summary>
        public static SystemComponent GetSystemComponent(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetSystemComponentAsync(id, query, cancellationToken));
        }

        /// <summary>
        /// Retrieves components within a system component.
        /// </summary>
        public static List<Component> GetSystemComponentComponents(this IdRofusClient client, int parentId, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetSystemComponentComponentsAsync(parentId, query, cancellationToken));
        }
    }
}
