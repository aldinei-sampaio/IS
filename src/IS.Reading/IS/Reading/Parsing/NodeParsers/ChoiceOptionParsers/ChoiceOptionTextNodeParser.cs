using IS.Reading.Parsing.ArgumentParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionTextNodeParser : IChoiceOptionTextNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public ChoiceOptionTextNodeParser(IElementParser elementParser, IBalloonTextParser balloonTextParser)
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(balloonTextParser);
    }

    public string Name => "text";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
        if (myContext.ParsedText is null)
            return;

        var ctx = (IChoiceOptionParentParsingContext)parentParsingContext;
        ctx.Option.Text = myContext.ParsedText;
    }
}
