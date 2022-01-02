using IS.Reading.Parsing.ConditionParsers;
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
        var result = conditionParser.Parse(reader.Value);

        if (result.Condition is not null)
            return new WhenAttribute(result.Condition);

        parsingContext.LogError(reader, "Condição 'when' inválida. " + result.Message);
        return null;
    }
}
