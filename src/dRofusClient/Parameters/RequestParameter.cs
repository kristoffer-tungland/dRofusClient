namespace dRofusClient.Parameters;

public record RequestParameter(string Name, string Value, bool ExcludeDollar = false)
{
    public RequestParameter(string name, IEnumerable<string> values, bool excludeDollar = false) :
        this(name, values.ToCommaSeparated(), excludeDollar)
    { }

    public override string ToString()
    {
        return ExcludeDollar ? $"{Name}={Value}" : $"${Name}={Value}";
    }
}