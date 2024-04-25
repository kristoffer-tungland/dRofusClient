namespace dRofusClient.Options;

public record dRofusBodyPostOptions(string Body) : dRofusOptionsBase
{
    public override void AddParametersToRequest(List<dRofusRequestParameter> parameters)
    {
    }
}