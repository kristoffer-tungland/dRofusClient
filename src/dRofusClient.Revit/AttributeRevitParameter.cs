using Autodesk.Revit.DB;

namespace dRofusClient.Revit;

public class AttributeRevitParameter
{
    public required string ParameterName { get; init; }
    public required List<InternalDefinition> Definitions { get; init; }
    public bool HasMultipleDefinitions => Definitions.Count > 1;

    public int? GetValue(Element element)
    {
        foreach (var definition in Definitions)
        {
            var value = GetParameterValueAsInt(element, definition);
            if (value.HasValue)
                return value;
        }   

        return null;
    }

    private int? GetParameterValueAsInt(Element element, Definition definition)
    {
        var parameter = element.get_Parameter(definition);

        if (parameter is null)
            return null;

        return ParseParameterValue(parameter);
    }

    private int? ParseParameterValue(Parameter p)
    {
        var parameterValue = GetParameterValue(p);

        if (int.TryParse(parameterValue, out var id))
        {
            return id;
        }

        return null;
    }

    private static string? GetParameterValue(Parameter p)
    {
        switch (p.StorageType)
        {
            case StorageType.String:
                var stringValue = p.AsString();

                if (stringValue == "0")
                    return null;

                return !string.IsNullOrWhiteSpace(stringValue) ? stringValue : null;
            case StorageType.Integer:
                var integerValue = p.AsInteger();
                return integerValue > 0 ? integerValue.ToString() : null;
            case StorageType.Double:
                var doubleValue = p.AsDouble();
                return doubleValue > 0 ? doubleValue.ToString() : null;
            case StorageType.ElementId:
                return p.AsElementId().ToString();
            default:
                return null;
        }
    }

    public bool SetValue(Element element, int occurenceId)
    {
        foreach (var definition in Definitions)
        {
            using var parameter = element.get_Parameter(definition);
            if (parameter is null)
                continue;

            if (parameter.StorageType == StorageType.String)
            {
                if (parameter.Set(occurenceId.ToString()))
                    return true;
            }
            if (parameter.StorageType == StorageType.Integer)
            {
                if (parameter.Set(occurenceId))
                    return true;
            }
        }

        return false;
    }

    public Element? GetElementWithOccurenceId(Document document, int occurenceId)
    {
        using var collector = new FilteredElementCollector(document)
            .WhereElementIsNotElementType()
            .WherePasses(CreateEqualsParameterFilter(occurenceId));

        return collector.FirstElement();
    }

    public FilteredElementCollector CreateCollector(Document document)
    {
        return new FilteredElementCollector(document)
            .WhereElementIsNotElementType()
            .WherePasses(CreateHasValueParameterFilter());
    }

    public FilteredElementCollector ApplyHasValueFilter(FilteredElementCollector collector)
    {
        collector.WhereElementIsNotElementType()
            .WherePasses(CreateHasValueParameterFilter());

        return collector;
    }

    public ElementFilter CreateHasValueParameterFilter()
    {
        var rules = new List<FilterRule>();

        foreach (var definition in Definitions)
        {
            var rule = ParameterFilterRuleFactory.CreateHasValueParameterRule(definition.Id);
            rules.Add(rule);
        }

        return new ElementParameterFilter(rules);
    }

    public ElementFilter CreateEqualsParameterFilter(int occurenceId)
    {
        var rules = new List<FilterRule>();
        foreach (var definition in Definitions)
        {
            var rule = ParameterFilterRuleFactory.CreateEqualsRule(definition.Id, occurenceId);
            rules.Add(rule);
        }
        return new ElementParameterFilter(rules);
    }
}