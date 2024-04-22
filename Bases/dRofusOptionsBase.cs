using System.Linq;
using dRofusClient.Parameters;
// ReSharper disable InconsistentNaming

namespace dRofusClient.Bases;

public abstract record dRofusOptionsBase
{
    public abstract void AddParametersToRequest(List<dRofusRequestParameter> parameters);

    public string? GetParameters()
    {
        var parameters = new List<dRofusRequestParameter>();
        AddParametersToRequest(parameters);

        return parameters.Any() ? string.Join("&", parameters) : null;
    }
}