using dRofusClient.Options;
using dRofusClient.Revit.Utils;

namespace dRofusClient.AttributeConfigurations
{
    /// <summary>
    /// Synchronous extension methods for working with Attribute Configuration resources using the IdRofusClient.
    /// </summary>
    public static class dRofusClientRevitAttributeConfigurationExtensions
    {
        /// <summary>
        /// Retrieves a list of attribute configurations matching the specified query.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="options">The query parameters for filtering and paging.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="AttributeConfiguration"/> objects.</returns>
        public static List<AttributeConfiguration> GetAttributeConfigurations(this IdRofusClient client, ListQuery? options = default, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetAttributeConfigurationsAsync(options, cancellationToken)
            );
        }

        /// <summary>
        /// Retrieves a list of attribute configurations by type and user availability.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="attributeConfigType">The attribute configuration type.</param>
        /// <param name="availableToUsers">Whether the configuration is available to users.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="AttributeConfiguration"/> objects.</returns>
        public static List<AttributeConfiguration> GetAttributeConfigurations(this IdRofusClient client, AttributeConfigType attributeConfigType, bool? availableToUsers = null, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetAttributeConfigurationsAsync(attributeConfigType, availableToUsers, cancellationToken)
            );
        }

        /// <summary>
        /// Retrieves a single attribute configuration by its ID.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="attributeConfigurationId">The ID of the attribute configuration.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="AttributeConfiguration"/> object with the specified ID, or null if not found.</returns>
        public static AttributeConfiguration? GetAttributeConfiguration(this IdRofusClient client, int attributeConfigurationId, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetAttributeConfigurationAsync(attributeConfigurationId, cancellationToken)
            );
        }
    }
}
