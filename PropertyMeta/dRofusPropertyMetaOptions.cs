using dRofusClient.Bases;
using dRofusClient.Parameters;

namespace dRofusClient.PropertyMeta;

public record dRofusPropertyMetaOptions(int Depth = 0) : dRofusOptionsBase
{
    public override void AddParametersToRequest(List<dRofusRequestParameter> parameters)
    {
        if (Depth < 1)
            return;

        var depthParameter = new dRofusRequestParameter(nameof(Depth), Depth.ToString());
        parameters.Add(depthParameter);
    }
}