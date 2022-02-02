using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.NodeParsers.PersonParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public abstract class BalloonTextNodeParserBase : INodeParser
{
    private readonly IElementParser elementParser;
    private readonly IBalloonChildNodeParser childParser;
    
    public IElementParserSettings Settings { get; }

    public IElementParserSettings AggregationSettings { get; }

    public BalloonTextNodeParserBase(
        IElementParser elementParser,
        IBalloonChildNodeParser childParser,
        ISetNodeParser setNodeParser,
        IUnsetNodeParser unsetNodeParser
    )
    {
        this.elementParser = elementParser;
        this.childParser = childParser;
        Settings = ElementParserSettings.AggregatedNonRepeat(childParser);
        AggregationSettings = ElementParserSettings.Aggregated(childParser, setNodeParser, unsetNodeParser);
    }

    public BalloonTextNodeParserBase(
        IElementParser elementParser,
        IBalloonChildNodeParser childParser,
        IMoodNodeParser moodNodeParser,
        ISetNodeParser setNodeParser,
        IUnsetNodeParser unsetNodeParser
    )
    {
        this.elementParser = elementParser;
        this.childParser = childParser;
        Settings = ElementParserSettings.AggregatedNonRepeat(childParser);
        AggregationSettings = ElementParserSettings.Aggregated(
            childParser, 
            moodNodeParser, 
            setNodeParser,
            unsetNodeParser
        );
    }

    public string Name => childParser.Name;

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new ParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (reader.ReadState != ReadState.EndOfFile)
            await elementParser.ParseAsync(reader, parsingContext, myContext, AggregationSettings);

        var nonPauseNodes = new Stack<INode>();
        for (var n = myContext.Nodes.Count - 1; n >= 0; n--)
        {
            var currentNode = myContext.Nodes[n];
            if (currentNode is IPauseNode)
                break;
            nonPauseNodes.Push(currentNode);
            myContext.Nodes.RemoveAt(n);
        }

        if (myContext.Nodes.Count == 0)
        {
            parsingContext.LogError(reader, "Era esperado um elemento filho.");
            return;
        }

        var block = parsingContext.BlockFactory.Create(myContext.Nodes);
        var node = new BalloonNode(childParser.BalloonType, block);
        parentParsingContext.AddNode(node);
        
        while (nonPauseNodes.TryPop(out var nonPauseNode))
            parentParsingContext.AddNode(nonPauseNode);

        return;
    }
}