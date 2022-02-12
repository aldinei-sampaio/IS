﻿using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Parsing.NodeParsers.BackgroundParsers;
using IS.Reading.State;

namespace IS.Reading.Parsing.NodeParsers;

public class BackgroundNodeParser : IBackgroundNodeParser
{
    private readonly IElementParser elementParser;
    private readonly IBackgroundImageTextParser backgroundImageTextParser;

    public IElementParserSettings Settings { get; }

    public BackgroundNodeParser(
        IElementParser elementParser,
        IBackgroundImageTextParser backgroundImageTextParser,
        IBackgroundColorNodeParser backgroundColorNodeParser,
        IBackgroundLeftNodeParser backgroundLeftNodeParser,
        IBackgroundRightNodeParser backgroundRightNodeParser,
        IBackgroundScrollNodeParser backgroundScrollNodeParser,
        IPauseNodeParser pauseNodeParser
    )
    {
        this.elementParser = elementParser;
        this.backgroundImageTextParser = backgroundImageTextParser;
        Settings = new ElementParserSettings.Aggregated(
            backgroundColorNodeParser,
            backgroundLeftNodeParser,
            backgroundRightNodeParser,
            backgroundScrollNodeParser,
            pauseNodeParser
        );
    }

    public bool IsArgumentRequired => false;

    public string Name => "background";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var context = new ParentParsingContext();

        if (!string.IsNullOrWhiteSpace(reader.Argument))
        {
            var result = backgroundImageTextParser.Parse(reader.Argument);
            if (!result.IsOk)
            {
                parsingContext.LogError(reader, result.ErrorMessage);
                return;
            }

            var state = new BackgroundState(result.Value, BackgroundType.Image, BackgroundPosition.Left);
            context.AddNode(new BackgroundNode(state));
            context.AddNode(new ScrollNode());
        }

        await elementParser.ParseAsync(reader, parsingContext, context, Settings);

        if (context.Nodes.Count == 0)
        {
            parsingContext.LogError(reader, "Nome de imagem ou elemento filho era esperado.");
            return;
        }

        var block = parsingContext.BlockFactory.Create(context.Nodes);
        parentParsingContext.AddNode(new BlockNode(block));
        parsingContext.RegisterDismissNode(DismissNode);
    }

    public INode DismissNode { get; } 
        = new BackgroundNode(BackgroundState.Empty);
   
}
