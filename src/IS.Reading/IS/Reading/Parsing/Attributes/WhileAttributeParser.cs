using System.Xml;

namespace IS.Reading.Parsing.Attributes;

public class WhileAttributeParser : IWhileAttributeParser
{
    private readonly IConditionParser conditionParser;

    public WhileAttributeParser(IConditionParser conditionParser)
    {
        this.conditionParser = conditionParser;
    }

    public string ElementName => "while";

    public IAttribute? Parse(XmlReader reader, IParsingContext parsingContext)
    {
        var condition = conditionParser.Parse(reader.Value);

        if (condition is not null)
            return new WhileAttribute(condition);

        parsingContext.LogError(reader, "Condição 'while' inválida.");
        return null;
    }
}
