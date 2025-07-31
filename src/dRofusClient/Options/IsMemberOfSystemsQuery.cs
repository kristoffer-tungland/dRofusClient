namespace dRofusClient.Options;

/// <summary>
/// Query options for the is-member-of-systems endpoint.
/// </summary>
public record IsMemberOfSystemsQuery : ListQuery
{
    public bool? IncludeSubs { get; init; }

    public override void AddParametersToRequest(List<RequestParameter> parameters)
    {
        base.AddParametersToRequest(parameters);
        if (IncludeSubs is not null)
        {
            var val = IncludeSubs.Value ? "true" : "false";
            parameters.Add(new RequestParameter("includeSubs", val, true));
        }
    }
}
