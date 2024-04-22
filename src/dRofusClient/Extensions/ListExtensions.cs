namespace dRofusClient.Extensions;

internal static class ListExtensions
{
    internal static string ToCommaSeparated(this IEnumerable<string> list)
    {
        return string.Join(",", list);
    }

    internal static void AddIfNotNull<T>(this List<T> list, T? item)
    {
        if (item is null)
            return;

        list.Add(item);
    }
}