using IS.Reading.Navigation;
using IS.Reading.Parsing.NodeParsers;

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
        IMainCharacterNodeParser mainCharacterNodeParser,
        IPersonNodeParser personNodeParser,
        INarrationNodeParser narrationNodeParser,
        ITutorialNodeParser tutorialNodeParser,
        ISetNodeParser setNodeParser
    )
    {
        this.elementParser = elementParser;
        Settings = new ElementParserSettings.NoBlock(
            musicNodeParser,
            backgroundNodeParser,
            blockNodeParser,
            pauseNodeParser,
            mainCharacterNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser,
            setNodeParser
        );
    }

    public async Task<List<INode>> ParseAsync(IDocumentReader reader, IParsingContext parsingContext)
    {
        var context = new ParentParsingContext();

        await elementParser.ParseAsync(reader, parsingContext, context, Settings);

        if (context.Nodes.Count == 0)
            parsingContext.LogError(reader, "Elemento filho era esperado.");

        return context.Nodes;
    }
}
