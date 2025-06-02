using System.Text.RegularExpressions;

namespace dRofusClient.Extensions;

internal static class dRofusPropertyToFieldExtensions
{
    private static readonly Regex _regex = new("(?<=[a-z])([A-Z])");

    public static string ToSnakeCase(this string field)
    {
        return _regex.Replace(field, "_$1").ToLower();
    }
}