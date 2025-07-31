
// ReSharper disable InconsistentNaming

namespace dRofusClient.Bases;

public abstract record RequestBase
{
    public abstract void AddParametersToRequest(List<RequestParameter> parameters);

    public string? GetParameters()
    {
        var parameters = new List<RequestParameter>();
        AddParametersToRequest(parameters);

        return parameters.Any() ? string.Join("&", parameters) : null;
    }
}

public abstract record RequestBodyBase : RequestBase
{
    public abstract string GetBody();
    
    public abstract string Accept { get; }
}