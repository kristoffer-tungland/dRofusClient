namespace dRofusClient;

public static class dRofusFieldsOptionsExtensions
{
    public static TOption Select<TOption>(this TOption options, string field)
        where TOption : dRofusFieldsOptions
    {
        return options.Select([field]);
    }

    public static TOption Select<TOption>(this TOption options, IEnumerable<string> fields)
        where TOption : dRofusFieldsOptions
    {
        options._fieldsToSelect.AddRange(fields.Select(x => x.ToLowerUnderscore()));
        return options;
    }
}