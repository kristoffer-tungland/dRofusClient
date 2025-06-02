using dRofusClient.Occurrences;
using dRofusClient.Options;
using dRofusClient.Revit.Utils;

namespace dRofusClient.Revit.Occurrences
{
    public static class dRofusClientRevitOccurenceExtensions
    {
        public static List<dRofusOccurence> GetOccurrences(this IdRofusClient client,
            dRofusListOptions options,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetOccurrencesAsync(options, cancellationToken)
            );
        }

        public static dRofusOccurence CreateOccurrence(this IdRofusClient client,
            dRofusOccurence occurence,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.CreateOccurrenceAsync(occurence, cancellationToken)
            );
        }

        public static dRofusOccurence GetOccurrence(this IdRofusClient client,
            int id,
            dRofusFieldsOptions options,
            CancellationToken cancellationToken = default)
        {
            return AsyncUtil.RunSync(() =>
                client.GetOccurrenceAsync(id, options, cancellationToken)
            );
        }

        public static dRofusOccurence UpdateOccurrence(this IdRofusClient client,
            dRofusOccurence occurence,
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