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

    public string Name => "enablewhen";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
        if (myContext.ParsedText is null)
            return;

        var condition = conditionParser.Parse(myContext.ParsedText);
        if (condition is null)
        {
            parsingContext.LogError(reader, "Condição inválida.");
            return;
        }
        var ctx = (ChoiceOptionParentParsingContext)parentParsingContext;
        ctx.Option.EnabledWhen = condition;
    }
}
