using System.Text.RegularExpressions;

namespace dRofusClient.Extensions;

internal static class dRofusPropertyToFieldExtensions
{
    static readonly Regex _regex = new("(?<=[a-z])([A-Z])");

    public static string ToLowerUnderscore(this string field)
    {
        return _regex.Replace(field, "_$1").ToLower();
    }
}