namespace dRofusClient.Options;

public record ItemQuery : RequestBase
{
    internal readonly List<string> _fieldsToSelect = [];

    private RequestParameter? GetSelectParameters()
    {
        return !_fieldsToSelect.Any() ? null : new RequestParameter("select", _fieldsToSelect);
    }

    public override void AddParametersToRequest(List<RequestParameter> parameters)
    {
        parameters.AddIfNotNull(GetSelectParameters());
    }

    public void AddFieldsToSelect(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
            throw new ArgumentException("Field cannot be null or empty.", nameof(field));

        _fieldsToSelect.Add(field.ToSnakeCase());
    }
}