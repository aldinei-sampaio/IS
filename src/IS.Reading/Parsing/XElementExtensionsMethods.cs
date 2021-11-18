using IS.Reading.Conditions;
using System.Linq;
using System.Xml.Linq;

namespace IS.Reading.Parsing;

public static class XElementExtensionsMethods
{
    public static string? GetAttributeValue(this XElement element, string name)
    {
        var att = element
            .Attributes()
            .Where(i => string.Compare(i.Name.LocalName, name, true) == 0)
            .FirstOrDefault();

        return att?.Value;
    }

    public static bool TryGetAttributeValue(this XElement element, string name, out string value)
    {
        var att = element
            .Attributes()
            .Where(i => string.Compare(i.Name.LocalName, name, true) == 0)
            .FirstOrDefault();

        if (att is null)
        {
            value = string.Empty;
            return false;
        }

        value = att.Value;
        return true;
    }

    public static ICondition? ParseCondition(this XElement element)
    {
        if (element.TryGetAttributeValue("condition", out var value))
            return ConditionParser.Parse(value);

        return null;
    }
}
