using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
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
        Settings = new ElementParserSettings(
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

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);

        if (!string.IsNullOrWhiteSpace(parsed.Text))
        {
            var block = new Block();
            var state = new BackgroundState(parsed.Text, BackgroundType.Image, BackgroundPosition.Left);
            block.ForwardQueue.Enqueue(new BackgroundNode(state, null));
            block.ForwardQueue.Enqueue(new ScrollNode(null));
            return new BlockNode(block, parsed.When, parsed.While);
        }

        if (parsed.Block is null || parsed.Block.ForwardQueue.Count == 0)
        {
            parsingContext.LogError(reader, "Nome de imagem ou elemento filho era esperado.");
            return null;
        }

        return new BlockNode(parsed.Block, parsed.When, parsed.While);
    }

    public INode? DismissNode { get; } 
        = DismissNode<BackgroundNode>.Create(new(BackgroundState.Empty, null));
}
