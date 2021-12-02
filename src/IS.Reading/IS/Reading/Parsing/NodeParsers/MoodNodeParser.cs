using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class MoodNodeParser : IMoodNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public MoodNodeParser(
        IElementParser elementParser, 
        IMoodTextParser moodTextParser,
        ISpeechNodeParser speechNodeParser,
        IThoughtNodeParser thoughtNodeParser,
        IPauseNodeParser pauseNodeParser
    )
    {
        this.elementParser = elementParser;
        Settings = new ElementParserSettings(moodTextParser);

        Aggregation = new NodeAggregation(
            speechNodeParser, 
            thoughtNodeParser,
            pauseNodeParser
        );
    }

    public INode? Aggregate(IBlock block)
    {
        if (block.ForwardQueue.Count <= 1)
            return null;

        if (block.ForwardQueue.Dequeue() is not MoodNode mainNode)
            return null;

        if (mainNode.ChildBlock is null)
            throw new InvalidOperationException();

        var source = block.ForwardQueue;
        var target = mainNode.ChildBlock.ForwardQueue;

        while (source.TryDequeue(out var item))
            target.Enqueue(item);

        return mainNode;
    }

    public string Name => "mood";

    public INodeAggregation? Aggregation { get; }

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);

        if (parsed.Text is null)
            return null;

        var moodType = Enum.Parse<MoodType>(parsed.Text);

        return new MoodNode(moodType, new Block());
    }
}
