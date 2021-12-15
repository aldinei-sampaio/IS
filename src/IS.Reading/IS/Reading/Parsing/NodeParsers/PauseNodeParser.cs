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
        Settings = ElementParserSettings.Normal(whenAttributeParser, integerTextParser);
    }

    public string Name => "pause";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var parsedText = myContext.ParsedText;

        parsingContext.SceneContext.Reset();

        if (string.IsNullOrWhiteSpace(parsedText))
        {
            var node = new PauseNode(myContext.When);
            parentParsingContext.AddNode(node);
            return;
        }

        var value = int.Parse(parsedText);

        if (value <= 0)
        {
            parsingContext.LogError(reader, "O tempo de espera precisa ser maior que zero.");
            return;
        }

        if (value > 5000)
        {
            parsingContext.LogError(reader, "O tempo de espera não pode ser maior que 5000.");
            return;
        }

        var timedPauseNode = new TimedPauseNode(TimeSpan.FromMilliseconds(value), myContext.When);
        parentParsingContext.AddNode(timedPauseNode);
    }
}
