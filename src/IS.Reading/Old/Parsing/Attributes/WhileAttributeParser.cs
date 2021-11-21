using IS.Reading.Conditions;
using System.Xml;

namespace IS.Reading.Parsing.Attributes;

public class WhileAttributeParser : IWhileAttributeParser
{
    public string ElementName => "when";

    public IAttribute? Parse(XmlReader reader, IParsingContext parsingContext)
    {
        if (ConditionParser.TryParse(reader.Value, out var condition))
            return new WhileAttribute(condition);

        parsingContext.LogError(reader, "Condição 'while' inválida.");
        return null;
    }
}
