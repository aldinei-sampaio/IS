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
        ITutorialNodeParser tutorialNodeParser,
        ISetNodeParser setNodeParser,
        IUnsetNodeParser unsetNodeParser
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
            tutorialNodeParser,
            setNodeParser,
            unsetNodeParser
        );
    }

    public async Task<IBlock> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var context = new BlockParentParsingContext();

        await elementParser.ParseAsync(reader, parsingContext, context, Settings);

        if (context.Nodes.Count == 0)
            parsingContext.LogError(reader, "Elemento filho era esperado.");

        return new Block(context.Nodes);
    }
}
