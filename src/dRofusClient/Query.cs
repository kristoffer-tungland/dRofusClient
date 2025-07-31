namespace dRofusClient;

public static class Query
{
    public static ItemQuery Field() => new();
    public static ListQuery List() => new();
    public static MetadataQuery Metadata(int depth = 0) => new(depth);
}