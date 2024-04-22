using System.Collections.Generic;
using System.Threading.Tasks;

namespace dRofusClient.WindowsCredentials;

public static class DictionaryExtensions
{
    public static Task<Dictionary<string, string>> ToDictionaryTask(this IEnumerable<string> items)
    {
        return Task.FromResult(items.ToDictionary());
    }

    public static Dictionary<string, string> ToDictionary(this IEnumerable<string> items)
    {
        var result = new Dictionary<string, string>();
        foreach (var item in items)
        {
            if (result.ContainsKey(item))
                continue;

            result.Add(item, item);
        }

        return result;
    }
}