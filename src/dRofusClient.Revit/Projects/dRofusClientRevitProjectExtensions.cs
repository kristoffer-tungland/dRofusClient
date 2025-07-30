using dRofusClient.Revit.Utils;
using dRofusClient.Options;
using System.Threading;

namespace dRofusClient.Projects
{
    /// <summary>
    /// Synchronous extension methods for working with Project resources using the IdRofusClient.
    /// </summary>
    public static class dRofusClientRevitProjectExtensions
    {
        /// <summary>
        /// Retrieves the dRofus project.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="options">Optional query parameters for field selection.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="Project"/> object representing the dRofus project.</returns>
        public static Project GetProject(this IdRofusClient client, ItemQuery? options = null, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetProjectAsync(options, cancellationToken)
            );
        }
    }
}
