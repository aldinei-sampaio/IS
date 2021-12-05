﻿using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionIconNodeParser : IChoiceOptionIconNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public ChoiceOptionIconNodeParser(IElementParser elementParser, INameTextParser textParser)
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(textParser);
    }

    public string Name => "icon";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
        if (myContext.ParsedText is null)
            return;

        var ctx = (ChoiceOptionParentParsingContext)parentParsingContext;
        ctx.Option.ImageName = myContext.ParsedText;
    }
}
