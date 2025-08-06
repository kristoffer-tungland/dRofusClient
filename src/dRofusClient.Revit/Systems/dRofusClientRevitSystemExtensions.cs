using dRofusClient.Options;
using dRofusClient.Revit.Utils;
using dRofusClient.ApiLogs;

namespace dRofusClient.Systems
{
    /// <summary>
    /// Synchronous extension methods for working with System resources using the IdRofusClient.
    /// </summary>
    public static class dRofusClientRevitSystemExtensions
    {
        /// <summary>
        /// Retrieves logs for all systems.
        /// </summary>
        public static List<ApiLogs.SystemLog> GetSystemLogs(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetSystemLogsAsync(query, cancellationToken));
        }

        /// <summary>
        /// Retrieves logs for a specific system.
        /// </summary>
        public static List<ApiLogs.SystemLog> GetSystemLogs(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetSystemLogsAsync(id, query, cancellationToken));
        }
        
        /// <summary>
        /// Retrieves a list of systems.
        /// </summary>
        public static List<SystemInstance> GetSystems(this IdRofusClient client, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetSystemsAsync(query, cancellationToken));
        }
        
        /// <summary>
        /// Retrieves a specific system.
        /// </summary>
        public static SystemInstance GetSystem(this IdRofusClient client, int id, ItemQuery? query = default, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetSystemAsync(id, query, cancellationToken));
        }
        
        /// <summary>
        /// Updates a system.
        /// </summary>
        public static SystemInstance UpdateSystem(this IdRofusClient client, SystemInstance system, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.UpdateSystemAsync(system, cancellationToken));
        }
        
        /// <summary>
        /// Deletes a system by id.
        /// </summary>
        public static void DeleteSystem(this IdRofusClient client, int id, CancellationToken cancellationToken = default)
        {
            AsyncUtil.RunSync(() => client.DeleteSystemAsync(id, cancellationToken));
        }
        
        /// <summary>
        /// Retrieves components for a specific system.
        /// </summary>
        public static List<SystemComponents.Component> GetSystemComponents(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetSystemComponentsAsync(id, query, cancellationToken));
        }
        
        /// <summary>
        /// Retrieves files for a system.
        /// </summary>
        public static List<Files.FileDetails> GetSystemFiles(this IdRofusClient client, int id, ListQuery query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() => client.GetSystemFilesAsync(id, query, cancellationToken));
        }
    }
}

