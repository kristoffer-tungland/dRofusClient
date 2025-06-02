using System.Text.Json;

namespace dRofusClient.Options;

public record dRofusPatchOptions : dRofusOptionsBodyBase
{
    public override string Accept => "application/merge-patch+json";
    public required string Body { get; init; }
    public Dictionary<string, object>? StatusFields { get; init; }

    public override void AddParametersToRequest(List<dRofusRequestParameter> parameters)
    {
    }

    public override string GetBody()
    {
        return Body;
    }
}