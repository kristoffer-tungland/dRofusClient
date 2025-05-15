namespace dRofusClient.Options;

public record dRofusFieldsOptions : dRofusOptionsBase
{
    internal readonly List<string> _fieldsToSelect = [];

    dRofusRequestParameter? GetSelectParameters()
    {
        return !_fieldsToSelect.Any() ? null : new dRofusRequestParameter("select", _fieldsToSelect);
    }

    public override void AddParametersToRequest(List<dRofusRequestParameter> parameters)
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