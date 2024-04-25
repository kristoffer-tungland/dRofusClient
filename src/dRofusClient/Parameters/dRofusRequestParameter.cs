namespace dRofusClient.Parameters;

public record dRofusRequestParameter(string Name, string Value, bool ExcludeDollar = false)
{
    public dRofusRequestParameter(string name, IEnumerable<string> values, bool excludeDollar = false) :
        this(name, values.ToCommaSeparated(), excludeDollar)
    { }

    public override string ToString()
    {
        return ExcludeDollar ? $"{Name}={Value}" : $"${Name}={Value}";
    }
}