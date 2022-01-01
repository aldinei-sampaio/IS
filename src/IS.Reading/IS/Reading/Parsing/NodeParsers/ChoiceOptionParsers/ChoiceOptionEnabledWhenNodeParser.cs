using IS.Reading.Parsing.ConditionParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionEnabledWhenNodeParser : IChoiceOptionEnabledWhenNodeParser
{
    private readonly IElementParser elementParser;
    private readonly IConditionParser conditionParser;

    public IElementParserSettings Settings { get; }

    public ChoiceOptionEnabledWhenNodeParser(IElementParser elementParser, IBalloonTextParser textParser, IConditionParser conditionParser)
    {
        this.elementParser = elementParser;
        this.conditionParser = conditionParser;
        Settings = ElementParserSettings.Normal(textParser);
    }

    public string Name => "enabledwhen";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
        if (myContext.ParsedText is null)
            return;

        var result = conditionParser.Parse(myContext.ParsedText);
        if (result.Condition is null)
        {
            parsingContext.LogError(reader, "Condição 'enabledwhen' inválida. " + result.Message);
            return;
        }
        var ctx = (IChoiceOptionParentParsingContext)parentParsingContext;
        ctx.Option.EnabledWhen = result.Condition;
    }
}
