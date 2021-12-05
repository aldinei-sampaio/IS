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
        IMusicNodeParser musicNodeParser,
        IBackgroundNodeParser backgroundNodeParser,
        IBlockNodeParser blockNodeParser,
        IPauseNodeParser pauseNodeParser,
        IProtagonistNodeParser protagonistNodeParser,
        IPersonNodeParser personNodeParser,
        INarrationNodeParser narrationNodeParser,
        ITutorialNodeParser tutorialNodeParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(
            musicNodeParser,
            backgroundNodeParser,
            blockNodeParser,
            pauseNodeParser,
            protagonistNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser
        );
    }

    public async Task<IBlock> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var context = new BlockParentParsingContext();

        await elementParser.ParseAsync(reader, parsingContext, context, Settings);

        if (context.Block.ForwardQueue.Count == 0)
            parsingContext.LogError(reader, "Elemento filho era esperado.");

        return context.Block;
    }

    public class RootBlockContext : IParentParsingContext
    {
        public Block Block { get; } = new();

        public void AddNode(INode node)
            => Block.ForwardQueue.Enqueue(node);
    }
}
