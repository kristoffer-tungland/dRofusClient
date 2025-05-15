using Autodesk.Revit.DB;
using dRofusClient.AttributeConfigurations;

namespace dRofusClient.Revit;

public static class AttributeRevitParameterExtensions
{
    public static AttributeRevitParameter GetDRofusKeyParameter(this Document document, dRofusAttributeConfiguration configuration)
    {
        var parameterName = configuration.Elements?.FirstOrDefault(x => x.Direction == "Key")?.ExternalAttributeId
            ?? throw new InvalidOperationException("Failed to get key parameter name from configuration");

        return document.GetDRofusAttributeRevitParameter(parameterName);
    }

    public static AttributeRevitParameter GetDRofusAttributeRevitParameter(this Document document, string parameterName)
    {
        var definitions = new List<InternalDefinition>();

        using var iterator = document.ParameterBindings.ForwardIterator();

        while (iterator.MoveNext())
        {
            if (iterator.Key is not InternalDefinition definition)
                continue;

            if (definition.Name == parameterName)
            {
                definitions.Add(definition);
                break;
            }
        }

        return new AttributeRevitParameter
        {
            ParameterName = parameterName,
            Definitions = definitions
        };
    }

    public static long? GetOccurenceId(this AttributeRevitParameter parameter, Element element)
    {
        return parameter.GetValue(element);
    }

    public static bool SetOccurenceId(this AttributeRevitParameter parameter, Element element, int occurenceId)
    {
        return parameter.SetValue(element, occurenceId);
    }
}
