using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class PersonNodeParser : IPersonNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

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
        Settings = new ElementParserSettings(nameTextParser);

        Aggregation = new NodeAggregation(
            speechNodeParser, 
            thoughtNodeParser,
            moodNodeParser,
            pauseNodeParser
        );
    }

    public INode? Aggregate(IBlock block)
    {
        if (block.ForwardQueue.Count <= 1)
            return null;

        if (block.ForwardQueue.Dequeue() is not PersonNode mainNode)
            return null;

        if (mainNode.ChildBlock is null)
            throw new InvalidOperationException();

        var source = block.ForwardQueue;
        var target = mainNode.ChildBlock.ForwardQueue;

        while (source.TryDequeue(out var item))
            target.Enqueue(item);

        return mainNode;
    }

    public string Name => "person";

    public INodeAggregation? Aggregation { get; }

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);

        if (parsed.Text is null)
            return null;

        if (parsed.Text.Length == 0)
        {
            parsingContext.LogError(reader, "Era esperado o nome do personagem.");
            return null;
        }

        return new PersonNode(parsed.Text, new Block());
    }
}
