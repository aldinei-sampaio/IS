﻿using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.State;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundRightNodeParser : IBackgroundRightNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public BackgroundRightNodeParser(
        IElementParser elementParser,
        IWhenAttributeParser whenAttributeParser,
        IBackgroundImageTextParser backgroundImageTextParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(whenAttributeParser, backgroundImageTextParser);
    }

    public string Name => "right";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (myContext.ParsedText is null)
            return;

        if (myContext.ParsedText.Length == 0)
        {
            parsingContext.LogError(reader, "Era esperado o nome da imagem.");
            return;
        }

        var state = new BackgroundState(myContext.ParsedText, BackgroundType.Image, BackgroundPosition.Right);
        var node = new BackgroundNode(state, myContext.When);
        parentParsingContext.AddNode(node);
    }
}