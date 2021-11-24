using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class BackgroundRightNodeParser : IBackgroundRightNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public BackgroundRightNodeParser(
        IElementParser elementParser,
        IWhenAttributeParser whenAttributeParser,
        IBackgroundImageTextParser backgroundImageTextParser
    )
    {
        this.elementParser = elementParser;
        Settings = new ElementParserSettings(whenAttributeParser, backgroundImageTextParser);
    }

    public string ElementName => "right";

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);

        if (parsed.Text is null)
            return null;

        if (parsed.Text.Length == 0)
        {
            parsingContext.LogError(reader, "Era esperado o nome da imagem.");
            return null;
        }

        return new BackgroundRightNode(parsed.Text, parsed.When);
    }
}
