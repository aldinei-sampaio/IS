using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class BlockNodeParser : IBlockNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public BlockNodeParser(
        IElementParser elementParser,
        IWhenAttributeParser whenAttributeParser,
        IWhileAttributeParser whileAttributeParser,
        IBackgroundNodeParser backgroundNodeParser,
        IPauseNodeParser pauseNodeParser
    )
    {
        this.elementParser = elementParser;
        Settings = new ElementParserSettings(
            whenAttributeParser, 
            whileAttributeParser,
            backgroundNodeParser,
            pauseNodeParser
        );
        Settings.ChildParsers.Add(this);
    }

    public string Name => "do";

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);

        if (parsed.Block is null || parsed.Block.ForwardQueue.Count == 0)
        {
            parsingContext.LogError(reader, "Elemento filho era esperado.");
            return null;
        }

        return new BlockNode(parsed.Block, parsed.When, parsed.While);
    }
}
