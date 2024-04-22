namespace dRofusClient.Extensions;

internal static class dRofusOrderByExtensions
{
    internal static string ToQuery(this dRofusOrderBy orderBy)
    {
        return orderBy switch
        {
            dRofusOrderBy.Ascending => "asc",
            dRofusOrderBy.Descending => "desc",
            _ => throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, null)
        };
    }
}