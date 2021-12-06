using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public abstract class BalloonTextNodeParserBase : INodeParser
{
    private readonly IElementParser elementParser;
    
    public IBalloonTextChildNodeParser ChildParser { get; }

    public IElementParserSettings AggregationSettings { get; }

    public BalloonTextNodeParserBase(
        IElementParser elementParser,
        IBalloonTextChildNodeParser childParser
    )
    {
        this.elementParser = elementParser;
        ChildParser = childParser;
        AggregationSettings = ElementParserSettings.Aggregated(childParser);
    }

    public string Name => ChildParser.Name;

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new BlockParentParsingContext();
        await ChildParser.ParseAsync(reader, parsingContext, myContext);

        if (reader.ReadState != ReadState.EndOfFile)
            await elementParser.ParseAsync(reader, parsingContext, myContext, AggregationSettings);

        var node = new BalloonNode(ChildParser.BalloonType, myContext.Block);
        parentParsingContext.AddNode(node);
    }
}