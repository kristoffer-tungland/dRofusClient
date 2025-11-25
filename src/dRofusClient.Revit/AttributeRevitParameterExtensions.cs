using Autodesk.Revit.DB;
using dRofusClient.AttributeConfigurations;

namespace dRofusClient.Revit;

public static class AttributeRevitParameterExtensions
{
    public static AttributeRevitParameter GetDRofusKeyParameter(this AttributeConfiguration configuration, Document document)
    {
        var keyConfigElement = configuration.Elements?.FirstOrDefault(x => x.Direction == AttributeConfigurationDirection.Key)
            ?? throw new InvalidOperationException("Failed to get key parameter name from configuration");

        return keyConfigElement.GetDRofusAttributeRevitParameter(document);
    }

    public static List<AttributeRevitParameter> GetDRofusAttributeRevitParameters(this AttributeConfiguration configuration, Document document)
    {
        var attributeRevitParameters = new List<AttributeRevitParameter>();
        if (configuration.Elements is null)
            return attributeRevitParameters;

        return configuration.Elements.GetDRofusAttributeRevitParameters(document);
    }

    public static List<AttributeRevitParameter> GetDRofusAttributeRevitParameters(this List<AttributeConfigurationElement>? configuration, Document document)
    {
        var attributeRevitParameters = new List<AttributeRevitParameter>();
        if (configuration is null)
            return attributeRevitParameters;

        foreach (var attributeConfigElement in configuration)
        {
            var attributeRevitParameter = attributeConfigElement.GetDRofusAttributeRevitParameter(document);
            attributeRevitParameters.Add(attributeRevitParameter);
        }
        return attributeRevitParameters;
    }

    public static AttributeRevitParameter GetDRofusKeyParameter(this List<AttributeRevitParameter> parameters)
    {
        var keyParameter = parameters.FirstOrDefault(x => x.AttributeConfigElement.Direction == AttributeConfigurationDirection.Key)
            ?? throw new InvalidOperationException("Failed to get key parameter from list of parameters");
        return keyParameter;
    }

    public static IEnumerable<AttributeRevitParameter> GetToDrofusParameters(this List<AttributeRevitParameter> parameters)
    {
        return parameters
            .Where(x => x.AttributeConfigElement.Direction == AttributeConfigurationDirection.ToDrofus);
    }

    public static IEnumerable<AttributeRevitParameter> GetToRevitParameters(this List<AttributeRevitParameter> parameters)
    {
        return parameters
            .Where(x => x.AttributeConfigElement.Direction == AttributeConfigurationDirection.ToExternalApplication);
    }

    public static AttributeRevitParameter GetDRofusAttributeRevitParameter(this AttributeConfigurationElement attributeConfigElement, Document document)
    {
        var revitParameterName = attributeConfigElement.ExternalAttributeId
            ?? throw new InvalidOperationException("Failed to get parameter name from attribute configuration element");

        if (string.IsNullOrEmpty(revitParameterName))
            throw new InvalidOperationException("Revit parameter name is null or empty");

        var definitions = new List<InternalDefinition>();

        if (revitParameterName.StartsWith("BuiltInParameter.", StringComparison.OrdinalIgnoreCase))
        {
            var builtInParameterDefinition = attributeConfigElement.ResolveBuiltInParameterDefinition(document)
                ?? throw new InvalidOperationException($"Failed to get built-in parameter name from string: {revitParameterName}");

            return builtInParameterDefinition;
        }
        else
        {
            using var collector = new FilteredElementCollector(document).OfClass(typeof(ParameterElement));

            using var iterator = collector.GetElementIterator();

            while (iterator.MoveNext())
            {
                var element = iterator.Current;

                if (element is not ParameterElement parameterElement)
                    continue;


                if (revitParameterName.EndsWith(parameterElement.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var definition = parameterElement.GetDefinition();

                    if (definition is null)
                        continue;

                    if (definition.Name.Equals(revitParameterName, StringComparison.OrdinalIgnoreCase) == false)
                        continue;

                    definitions.Add(parameterElement.GetDefinition());
                    break;
                }
            }
        }

        return new AttributeRevitParameter
        {
            ParameterName = revitParameterName,
            AttributeConfigElement = attributeConfigElement,
            Definitions = definitions
        };
    }

    public static int? GetOccurenceId(this AttributeRevitParameter parameter, Element element)
    {
        return parameter.GetValue(element);
    }

    public static bool SetOccurenceId(this AttributeRevitParameter parameter, Element element, int occurenceId)
    {
        return parameter.SetValue(element, occurenceId);
    }
}

public static class BuiltInParameterHelper
{
    public static AttributeRevitParameter? ResolveBuiltInParameterDefinition(this AttributeConfigurationElement attributeConfigElement, Document document)
    {
        var builtInParameterString = attributeConfigElement.ExternalAttributeId
            ?? throw new InvalidOperationException("Failed to get parameter name from attribute configuration element");

        var parts = builtInParameterString.Split('.');
        var enumValuePart = parts.Last();


        if (Enum.TryParse<BuiltInParameter>(enumValuePart, true, out var builtInParameter))
        {
            var displayName = LabelUtils.GetLabelFor(builtInParameter, document.Application.Language)
                ?? throw new InvalidOperationException($"Unable to get label for built in parameter {enumValuePart}");

            return new AttributeRevitParameter
            {
                ParameterName = displayName,
                AttributeConfigElement = attributeConfigElement,
                Definitions = [],
                BuiltInParameter = builtInParameter
            };
        }
        return null;
    }
}