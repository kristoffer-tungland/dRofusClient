namespace dRofusClient.Extensions;

internal static class dRofusPropertyToFieldExtensions
{
    public static string ToLowerUnderscore(this string field)
    {
        return field.ToLower();
    }
}