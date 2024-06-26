namespace dRofusClient.Options;

public record dRofusPropertyMetaOptions(int Depth = 0) : dRofusOptionsBase
{
    public override void AddParametersToRequest(List<dRofusRequestParameter> parameters)
    {
        if (Depth < 1)
            return;

        var depthParameter = new dRofusRequestParameter(nameof(Depth).ToSnakeCase(), Depth.ToString(), true);
        parameters.Add(depthParameter);
    }
}