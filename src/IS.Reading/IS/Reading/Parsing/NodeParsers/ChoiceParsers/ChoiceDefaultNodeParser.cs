using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceDefaultNodeParser : IChoiceDefaultNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public ChoiceDefaultNodeParser(IElementParser elementParser, INameTextParser textParser)
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(textParser);
    }

    public string Name => "default";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
        if (myContext.ParsedText is null)
            return;

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.Choice.Default = myContext.ParsedText;
    }
}
