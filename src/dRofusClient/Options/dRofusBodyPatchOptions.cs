namespace dRofusClient.Options;

public record dRofusBodyPatchOptions(string Body) : dRofusOptionsBase
{
    public override void AddParametersToRequest(List<dRofusRequestParameter> parameters)
    {
    }
}