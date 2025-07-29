namespace dRofusClient.Options;

public record MetadataQuery(int Depth = 0) : RequestBase
{
    public override void AddParametersToRequest(List<RequestParameter> parameters)
    {
        if (Depth < 1)
            return;

        var depthParameter = new RequestParameter(nameof(Depth).ToSnakeCase(), Depth.ToString(), true);
        parameters.Add(depthParameter);
    }
}