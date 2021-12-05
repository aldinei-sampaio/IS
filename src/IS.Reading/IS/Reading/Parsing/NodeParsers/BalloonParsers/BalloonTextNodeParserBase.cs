using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public abstract class BalloonTextNodeParserBase : INodeParser
{
    private readonly IElementParser elementParser;
    private readonly IBalloonTextChildNodeParser childParser;

    public IElementParserSettings AggregationSettings { get; }

    public BalloonTextNodeParserBase(
        IElementParser elementParser,
        IBalloonTextChildNodeParser childParser
    )
    {
        this.elementParser = elementParser;
        this.childParser = childParser;
        AggregationSettings = ElementParserSettings.Aggregated(childParser);
    }

    public string Name => childParser.Name;

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new BlockParentParsingContext();
        await childParser.ParseAsync(reader, parsingContext, myContext);

        if (reader.ReadState != ReadState.EndOfFile)
            await elementParser.ParseAsync(reader, parsingContext, myContext, AggregationSettings);

        var node = new BalloonNode(childParser.BalloonType, myContext.Block);
        parentParsingContext.AddNode(node);
    }
}