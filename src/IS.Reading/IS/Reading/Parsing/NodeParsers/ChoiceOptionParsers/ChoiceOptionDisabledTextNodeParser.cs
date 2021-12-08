﻿using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionDisabledTextNodeParser : IChoiceOptionDisabledTextNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public ChoiceOptionDisabledTextNodeParser(IElementParser elementParser, IBalloonTextParser textParser)
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(textParser);
    }

    public string Name => "disabledtext";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
        if (myContext.ParsedText is null)
            return;

        var ctx = (ChoiceOptionParentParsingContext)parentParsingContext;
        ctx.Option.DisabledText = myContext.ParsedText;
    }
}