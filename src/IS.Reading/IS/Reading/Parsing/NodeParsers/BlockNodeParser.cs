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
        IMusicNodeParser musicNodeParser,
        IBackgroundNodeParser backgroundNodeParser,
        IPauseNodeParser pauseNodeParser,
        IProtagonistNodeParser protagonistNodeParser,
        IPersonNodeParser personNodeParser,
        INarrationNodeParser narrationNodeParser,
        ITutorialNodeParser tutorialNodeParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(
            whenAttributeParser, 
            whileAttributeParser,
            musicNodeParser,
            backgroundNodeParser,
            pauseNodeParser,
            protagonistNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser
        );
        Settings.ChildParsers.Add(this);
    }

    public string Name => "do";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new BlockParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (myContext.Block.ForwardQueue.Count == 0)
        {
            parsingContext.LogError(reader, "Elemento filho era esperado.");
            return;
        }

        var node = new BlockNode(myContext.Block, myContext.When, myContext.While);
        parentParsingContext.AddNode(node);
    }
}
