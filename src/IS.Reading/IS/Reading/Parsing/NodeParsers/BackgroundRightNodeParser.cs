﻿using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.State;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

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
        Settings = new ElementParserSettings(whenAttributeParser, backgroundImageTextParser);
    }

    public string Name => "right";

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);

        if (parsed.Text is null)
            return null;

        if (parsed.Text.Length == 0)
        {
            parsingContext.LogError(reader, "Era esperado o nome da imagem.");
            return null;
        }

        var state = new BackgroundState(parsed.Text, BackgroundType.Image, BackgroundPosition.Right);

        return new BackgroundNode(state, parsed.When);
    }
}
