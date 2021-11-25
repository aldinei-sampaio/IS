using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class PauseNodeParser : IPauseNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public PauseNodeParser(
        IElementParser elementParser, 
        IWhenAttributeParser whenAttributeParser,
        IIntegerTextParser integerTextParser
    )
    {
        this.elementParser = elementParser;
        Settings = new ElementParserSettings(whenAttributeParser, integerTextParser);
    }

    public string ElementName => "pause";

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);

        if (string.IsNullOrEmpty(parsed.Text))
            return new PauseNode(null, parsed.When);

        var value = int.Parse(parsed.Text);
        return new PauseNode(TimeSpan.FromMilliseconds(value), parsed.When);
    }
}
