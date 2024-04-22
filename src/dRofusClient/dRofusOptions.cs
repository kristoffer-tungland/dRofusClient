namespace dRofusClient;

public static class dRofusOptions
{
    public static dRofusFieldsOptions Field() => new();
    public static dRofusListOptions List() => new();
    public static dRofusPropertyMetaOptions PropertyMeta(int depth = 0) => new(depth);
}