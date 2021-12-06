using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class MoodNodeParser : IMoodNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public IElementParserSettings AggregationSettings { get; }

    public MoodNodeParser(
        IElementParser elementParser, 
        IMoodTextParser moodTextParser,
        ISpeechNodeParser speechNodeParser,
        IThoughtNodeParser thoughtNodeParser,
        IPauseNodeParser pauseNodeParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(moodTextParser);

        AggregationSettings = ElementParserSettings.Aggregated(
            speechNodeParser, 
            thoughtNodeParser,
            pauseNodeParser
        );
    }

    public string Name => "mood";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var parsedText = myContext.ParsedText;

        if (parsedText is null)
            return;

        var moodType = Enum.Parse<MoodType>(parsedText);

        var aggContext = new BlockParentParsingContext();
        if (reader.ReadState != ReadState.EndOfFile)
            await elementParser.ParseAsync(reader, parsingContext, aggContext, AggregationSettings);

        var node = new MoodNode(moodType, aggContext.Block);
        parentParsingContext.AddNode(node);
    }
}
