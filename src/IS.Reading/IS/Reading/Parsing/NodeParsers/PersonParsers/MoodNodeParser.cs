using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class MoodNodeParser : IMoodNodeParser
{
    private readonly IElementParser elementParser;
    private readonly INodeParser childParser;

    public IElementParserSettings Settings { get; }

    public IElementParserSettings AggregationSettings { get; }

    public MoodNodeParser(
        IElementParser elementParser, 
        IMoodTextNodeParser moodTextNodeParser,
        ISpeechNodeParser speechNodeParser,
        IThoughtNodeParser thoughtNodeParser,
        IPauseNodeParser pauseNodeParser
    )
    {
        this.elementParser = elementParser;
        this.childParser = moodTextNodeParser;
        Settings = ElementParserSettings.NoRepeat(moodTextNodeParser);

        AggregationSettings = ElementParserSettings.Aggregated(
            speechNodeParser, 
            thoughtNodeParser,
            pauseNodeParser
        );
    }

    public string Name => childParser.Name;

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new BlockParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var parsedText = myContext.ParsedText;

        if (parsedText is null)
            return;

        var moodType = Enum.Parse<MoodType>(parsedText);

        if (reader.ReadState != ReadState.EndOfFile)
            await elementParser.ParseAsync(reader, parsingContext, myContext, AggregationSettings);

        var node = new MoodNode(moodType, myContext.Block);
        parentParsingContext.AddNode(node);
    }
}
