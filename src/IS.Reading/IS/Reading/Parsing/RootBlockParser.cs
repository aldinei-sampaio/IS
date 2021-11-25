using IS.Reading.Navigation;
using IS.Reading.Parsing.NodeParsers;
using System.Xml;

namespace IS.Reading.Parsing;

public class RootBlockParser : IRootBlockParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public RootBlockParser(
        IElementParser elementParser,
        IBackgroundNodeParser backgroundNodeParser,
        IBlockNodeParser blockNodeParser,
        IPauseNodeParser pauseNodeParser
    )
    {
        this.elementParser = elementParser;
        Settings = new ElementParserSettings(
            backgroundNodeParser,
            blockNodeParser,
            pauseNodeParser
        );
    }

    public async Task<IBlock?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);

        if (parsed.Block is null || parsed.Block.ForwardQueue.Count == 0)
        {
            parsingContext.LogError(reader, "Elemento filho era esperado.");
            return null;
        }

        return parsed.Block;
    }
}
