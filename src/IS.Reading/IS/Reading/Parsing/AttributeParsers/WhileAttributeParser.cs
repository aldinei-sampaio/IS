using IS.Reading.Parsing.ConditionParsers;
using System.Xml;

namespace IS.Reading.Parsing.AttributeParsers;

public class WhileAttributeParser : IWhileAttributeParser
{
    private readonly IConditionParser conditionParser;

    public WhileAttributeParser(IConditionParser conditionParser)
    {
        this.conditionParser = conditionParser;
    }

    public string Name => "while";

    public IAttribute? Parse(XmlReader reader, IParsingContext parsingContext)
    {
        var result = conditionParser.Parse(reader.Value);

        if (result.Condition is not null)
            return new WhileAttribute(result.Condition);

        parsingContext.LogError(reader, "Condição 'while' inválida. " + result.Message);
        return null;
    }
}
