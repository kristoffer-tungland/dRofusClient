using System.Text.Json;

namespace dRofusClient.Options;

public record dRofusPostOptions : dRofusOptionsBodyBase
{
    public override string Accept => "application/json";

    public required string Body { get; init; }
    public List<JsonProperty>? StatusFields { get; init; }

    public override void AddParametersToRequest(List<dRofusRequestParameter> parameters)
    {
    }

    public override string GetBody()
    {
        return Body;
    }
}