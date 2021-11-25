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

        if (string.IsNullOrWhiteSpace(parsed.Text))
            return new PauseNode(parsed.When);

        var value = int.Parse(parsed.Text);

        if (value <= 0)
        {
            parsingContext.LogError(reader, "O tempo de espera precisa ser maior que zero.");
            return null;
        }

        if (value > 5000)
        {
            parsingContext.LogError(reader, "O tempo de espera não pode ser maior que 5000.");
            return null;
        }

        return new TimedPauseNode(TimeSpan.FromMilliseconds(value), parsed.When);
    }
}
