namespace dRofusClient.Parameters;

public record dRofusRequestParameter(string Name, string Value)
{
    public dRofusRequestParameter(string Name, IEnumerable<string> Values) :
        this(Name, Values.ToCommaSeparated())
    { }

    public override string ToString()
    {
        return $"${Name}={Value}";
    }
}