using IS.Reading.Nodes;
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
        IBalloonChildNodeParser childParser
    )
    {
        this.elementParser = elementParser;
        this.childParser = childParser;
        Settings = ElementParserSettings.AggregatedNonRepeat(childParser);
        AggregationSettings = ElementParserSettings.Aggregated(childParser);
    }

    public string Name => childParser.Name;

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new BlockParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (reader.ReadState != ReadState.EndOfFile)
            await elementParser.ParseAsync(reader, parsingContext, myContext, AggregationSettings);

        var node = new BalloonNode(childParser.BalloonType, myContext.Block);
        parentParsingContext.AddNode(node);
    }
}