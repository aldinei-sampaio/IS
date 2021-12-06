using IS.Reading.Nodes;
using IS.Reading.Parsing.NodeParsers.PersonParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class PersonNodeParser : IPersonNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public IElementParserSettings AggregationSettings { get; }

    public PersonNodeParser(
        IElementParser elementParser, 
        INameTextParser nameTextParser,
        ISpeechNodeParser speechNodeParser,
        IThoughtNodeParser thoughtNodeParser,
        IMoodNodeParser moodNodeParser,
        IPauseNodeParser pauseNodeParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(nameTextParser);

        AggregationSettings = ElementParserSettings.Aggregated(
            speechNodeParser, 
            thoughtNodeParser,
            moodNodeParser,
            pauseNodeParser
        );
    }

    public string Name => "person";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var parsedText = myContext.ParsedText;

        if (parsedText is null)
            return;

        if (parsedText.Length == 0)
        {
            parsingContext.LogError(reader, "Era esperado o nome do personagem.");
            return;
        }

        if (reader.ReadState == ReadState.EndOfFile)
            return;

        var aggContext = new BlockParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, aggContext, AggregationSettings);

        if (aggContext.Block.ForwardQueue.Count == 0)
            return;

        var node = new PersonNode(parsedText, aggContext.Block);
        parentParsingContext.AddNode(node);
    }
}