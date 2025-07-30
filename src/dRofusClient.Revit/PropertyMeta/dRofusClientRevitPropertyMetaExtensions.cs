using dRofusClient.PropertyMeta;
using dRofusClient.Options;
using dRofusClient.Revit.Utils;

namespace dRofusClient.Revit.PropertyMeta
{
    /// <summary>
    /// Synchronous extension methods for working with Property Meta resources using the IdRofusClient.
    /// </summary>
    public static class dRofusClientRevitPropertyMetaExtensions
    {
        /// <summary>
        /// Retrieves a list of property meta information for the specified dRofus type.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="dRofusType">The dRofus type for which to retrieve property meta.</param>
        /// <param name="options">Optional metadata query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="dRofusPropertyMeta"/> objects.</returns>
        public static List<dRofusPropertyMeta> GetPropertyMeta(this IdRofusClient client,
            dRofusType dRofusType,
            MetadataQuery? options = default,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetPropertyMetaAsync(dRofusType, options, cancellationToken)
            );
        }
    }
}