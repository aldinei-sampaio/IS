using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.NodeParsers.BackgroundParsers;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.State;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class BackgroundNodeParser : IBackgroundNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public BackgroundNodeParser(
        IElementParser elementParser,
        IWhenAttributeParser whenAttributeParser,
        IBackgroundImageTextParser backgroundImageTextParser,
        IBackgroundColorNodeParser backgroundColorNodeParser,
        IBackgroundLeftNodeParser backgroundLeftNodeParser,
        IBackgroundRightNodeParser backgroundRightNodeParser,
        IBackgroundScrollNodeParser backgroundScrollNodeParser,
        IPauseNodeParser pauseNodeParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(
            whenAttributeParser, 
            backgroundImageTextParser,
            backgroundColorNodeParser,
            backgroundLeftNodeParser,
            backgroundRightNodeParser,
            backgroundScrollNodeParser,
            pauseNodeParser
        );
    }

    public string Name => "background";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var context = new BackgroundContext();
        await elementParser.ParseAsync(reader, parsingContext, context, Settings);

        if (!string.IsNullOrWhiteSpace(context.ParsedText))
        {
            var block = new Block();
            var state = new BackgroundState(context.ParsedText, BackgroundType.Image, BackgroundPosition.Left);
            block.ForwardQueue.Enqueue(new BackgroundNode(state, null));
            block.ForwardQueue.Enqueue(new ScrollNode(null));
            parentParsingContext.AddNode(new BlockNode(block, context.When, null));
            parsingContext.RegisterDismissNode(DismissNode);
            return;
        }

        if (context.Block is null || context.Block.ForwardQueue.Count == 0)
        {
            parsingContext.LogError(reader, "Nome de imagem ou elemento filho era esperado.");
            return;
        }

        parentParsingContext.AddNode(new BlockNode(context.Block, context.When, null));
        parsingContext.RegisterDismissNode(DismissNode);
    }

    public INode DismissNode { get; } 
        = DismissNode<BackgroundNode>.Create(new(BackgroundState.Empty, null));

    public class BackgroundContext : IParentParsingContext
    {
        public IBlock? Block { get; private set; }

        public void AddNode(INode node)
        {
            if (Block is null)
                Block = new Block();
            Block.ForwardQueue.Enqueue(node);
        }

        public string? ParsedText { get; set; }
        public ICondition? When { get; set; }
    }
}
