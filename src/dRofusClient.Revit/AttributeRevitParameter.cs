using Autodesk.Revit.DB;
using dRofusClient.AttributeConfigurations;

namespace dRofusClient.Revit;

public class AttributeRevitParameter
{
    public required string ParameterName { get; init; }

    public string Id => AttributeConfigElement.DrofusAttributeId
        ?? throw new InvalidOperationException("Attribute ID is null");

    public required AttributeConfigurationElement AttributeConfigElement { get; init; }
    public required List<InternalDefinition> Definitions { get; init; }
    public bool HasMultipleDefinitions => Definitions.Count > 1;

    public BuiltInParameter? BuiltInParameter { get; init; }

    public Parameter GetParameter(Element element)
    {
        if (BuiltInParameter.HasValue)
        {
            var parameter = element.get_Parameter(BuiltInParameter.Value);
            if (parameter is not null)
            {
                return parameter;
            }
        }
        foreach (var definition in Definitions)
        {
            var parameter = element.get_Parameter(definition);
            if (parameter is not null)
                return parameter;
        }   
        throw new InvalidOperationException($"Parameter '{ParameterName}' not found on element");
    }

    public int? GetValue(Element element)
    {
        if (BuiltInParameter.HasValue)
        {
            var parameter = element.get_Parameter(BuiltInParameter.Value);
            if (parameter is not null)
            {
                return ParseParameterValue(parameter);
            }
        }

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
        if (BuiltInParameter.HasValue)
        {
            using var parameter = element.get_Parameter(BuiltInParameter.Value);
            if (parameter is not null && TrySetParameterValue(parameter, occurenceId))
            {
                return true;
            }
        }

        foreach (var definition in Definitions)
        {
            using var parameter = element.get_Parameter(definition);
            if (parameter is not null && TrySetParameterValue(parameter, occurenceId))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TrySetParameterValue(Parameter parameter, int occurenceId)
    {
        if (parameter.IsReadOnly)
            return false;

        try
        {
            switch (parameter.StorageType)
            {
                case StorageType.String:
                    return parameter.Set(occurenceId.ToString());
                case StorageType.Integer:
                    return parameter.Set(occurenceId);
                case StorageType.Double:
                    return parameter.Set((double)occurenceId);
                case StorageType.ElementId:
#if R2014_OR_GREATER
                    return parameter.Set(new ElementId(occurenceId), false);
#else
                    return parameter.Set(new ElementId((long)occurenceId));
#endif
                default:
                    return false;
            }
        }
        catch
        {
            return false;
        }
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

        if (BuiltInParameter.HasValue)
        {
            var rule = ParameterFilterRuleFactory.CreateHasValueParameterRule(new ElementId(BuiltInParameter.Value));
            rules.Add(rule);
        }

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
        
        if (BuiltInParameter.HasValue)
        {
          var rule = ParameterFilterRuleFactory.CreateEqualsRule(new ElementId(BuiltInParameter.Value), occurenceId);
            rules.Add(rule);
        }
        
        foreach (var definition in Definitions)
        {
            var rule = ParameterFilterRuleFactory.CreateEqualsRule(definition.Id, occurenceId);
            rules.Add(rule);
        }
    
        return new ElementParameterFilter(rules);
    }
}