using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.State;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

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
        Settings = new ElementParserSettings(whenAttributeParser, colorTextParser);
    }

    public string ElementName => "color";

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);

        if (parsed.Text is null)
            return null;

        if (parsed.Text.Length == 0)
        {
            parsingContext.LogError(reader, "Era esperado o nome da cor.");
            return null;
        }

        var state = new BackgroundState(parsed.Text, BackgroundType.Color, BackgroundPosition.Undefined);

        return new BackgroundChangeNode(state, parsed.When);
    }
}
