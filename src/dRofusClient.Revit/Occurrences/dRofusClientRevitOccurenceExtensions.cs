using dRofusClient.Options;
using dRofusClient.Revit.Utils;

namespace dRofusClient.Occurrences
{
    /// <summary>
    /// Synchronous extension methods for working with Occurrence resources using the IdRofusClient.
    /// </summary>
    public static class dRofusClientRevitOccurenceExtensions
    {
        /// <summary>
        /// Retrieves a list of occurrences matching the specified query.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="options">The query parameters for filtering and paging.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of <see cref="Occurence"/> objects.</returns>
        public static List<Occurence> GetOccurrences(this IdRofusClient client,
            ListQuery options,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetOccurrencesAsync(options, cancellationToken)
            );
        }

        /// <summary>
        /// Creates a new occurrence with the specified parameters.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="occurenceToCreate">The occurrence to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created <see cref="Occurence"/> object.</returns>
        public static Occurence CreateOccurrence(this IdRofusClient client, CreateOccurence occurenceToCreate, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.CreateOccurrenceAsync(occurenceToCreate, cancellationToken)
            );
        }

        /// <summary>
        /// Retrieves a single occurrence by its ID.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="id">The ID of the occurrence.</param>
        /// <param name="options">Optional query parameters for field selection.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="Occurence"/> object with the specified ID.</returns>
        public static Occurence GetOccurrence(this IdRofusClient client,
            int id,
            ItemQuery options,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetOccurrenceAsync(id, options, cancellationToken)
            );
        }

        /// <summary>
        /// Updates an existing occurrence. If status fields are present, updates statuses as well.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="occurence">The occurrence to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated <see cref="Occurence"/> object.</returns>
        public static Occurence UpdateOccurrence(this IdRofusClient client,
            Occurence occurence,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.UpdateOccurrenceAsync(occurence, cancellationToken)
            );
        }

        /// <summary>
        /// Updates a single status field for a given occurrence.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="id">The ID of the occurrence.</param>
        /// <param name="query">The status patch request.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The <see cref="StatusPatchResult"/> of the update operation.</returns>
        public static StatusPatchResult UpdateOccurrenceStatus(this IdRofusClient client, int id, StatusPatchRequest query, CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.UpdateOccurrenceStatusAsync(id, query, cancellationToken)
            );
        }

        /// <summary>
        /// Deletes an occurrence by its ID.
        /// </summary>
        /// <param name="client">The dRofus client instance.</param>
        /// <param name="id">The ID of the occurrence to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public static void DeleteOccurrence(this IdRofusClient client,
            int id,
            CancellationToken cancellationToken = default)
        {
            AsyncUtil.RunSync(() =>
                client.DeleteOccurrenceAsync(id, cancellationToken)
            );
        }
    }
}