using System.Xml;

namespace IS.Reading.Parsing.AttributeParsers;

public class WhenAttributeParser : IWhenAttributeParser
{
    private readonly IConditionParser conditionParser;

    public WhenAttributeParser(IConditionParser conditionParser)
    {
        this.conditionParser = conditionParser;
    }

    public string Name => "when";

    public IAttribute? Parse(XmlReader reader, IParsingContext parsingContext)
    {
        var condition = conditionParser.Parse(reader.Value);

        if (condition is not null)
            return new WhenAttribute(condition);

        parsingContext.LogError(reader, "Condição 'when' inválida.");
        return null;
    }
}
