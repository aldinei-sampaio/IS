using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class BackgroundScrollNodeParser : IBackgroundScrollNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public BackgroundScrollNodeParser(IElementParser elementParser, IWhenAttributeParser whenAttributeParser)
    {
        this.elementParser = elementParser;
        Settings = new ElementParserSettings(whenAttributeParser);
    }

    public string ElementName => "scroll";

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);
        return new BackgroundScrollNode(parsed.When);
    }
}
