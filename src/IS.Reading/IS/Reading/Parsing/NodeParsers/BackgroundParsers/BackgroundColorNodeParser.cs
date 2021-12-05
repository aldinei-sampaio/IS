using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.State;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundColorNodeParser : IBackgroundColorNodeParser
{
    private readonly IElementParser elementParser;
    
    public IElementParserSettings Settings { get; }

    public BackgroundColorNodeParser(
        IElementParser elementParser, 
        IWhenAttributeParser whenAttributeParser,
        IColorTextParser colorTextParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(whenAttributeParser, colorTextParser);
    }

    public string Name => "color";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext(); 
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (myContext.ParsedText is null)
            return;

        if (myContext.ParsedText.Length == 0)
        {
            parsingContext.LogError(reader, "Era esperado o nome da cor.");
            return;
        }

        var state = new BackgroundState(myContext.ParsedText, BackgroundType.Color, BackgroundPosition.Undefined);
        var node = new BackgroundNode(state, myContext.When);
        parentParsingContext.AddNode(node);
    }
}
