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
    }
}

