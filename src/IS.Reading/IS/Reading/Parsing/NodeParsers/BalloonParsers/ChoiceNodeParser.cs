﻿using IS.Reading.Parsing.NodeParsers.ChoiceParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class ChoiceNodeParser : IChoiceNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public ChoiceNodeParser(
        IElementParser elementParser, 
        IChoiceTimeLimitNodeParser timeLimitNodeParser,
        IChoiceDefaultNodeParser defaultNodeParser,
        IChoiceOptionNodeParser optionNodeParser
    )
    {
        this.elementParser = elementParser;

        Settings = ElementParserSettings.Normal(
            timeLimitNodeParser,
            defaultNodeParser,
            optionNodeParser
        );
    }

    public string Name => "choice";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new ChoiceParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
        if (myContext.Choice.Options.Count == 0)
        {
            parsingContext.LogError(reader, "Nenhuma opção informada.");
            return;
        }

        var ctx = (BalloonTextChildParentParsingContext)parentParsingContext;
        ctx.ChoiceNode = myContext.Choice;
    }
}