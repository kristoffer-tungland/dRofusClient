using dRofusClient.Options;
using dRofusClient.Revit.Utils;

namespace dRofusClient.Occurrences
{
    public static class dRofusClientRevitOccurenceExtensions
    {
        public static List<Occurence> GetOccurrences(this IdRofusClient client,
            ListQuery options,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetOccurrencesAsync(options, cancellationToken)
            );
        }

        public static Occurence CreateOccurrence(this IdRofusClient client,
            Occurence occurence,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.CreateOccurrenceAsync(occurence, cancellationToken)
            );
        }

        public static Occurence GetOccurrence(this IdRofusClient client,
            int id,
            ItemQuery options,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetOccurrenceAsync(id, options, cancellationToken)
            );
        }

        public static Occurence UpdateOccurrence(this IdRofusClient client,
            Occurence occurence,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.UpdateOccurrenceAsync(occurence, cancellationToken)
            );
        }

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