﻿using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionIconNodeParser : IChoiceOptionIconNodeParser
{
    public INameArgumentParser NameArgumentParser { get; }

    public ChoiceOptionIconNodeParser(INameArgumentParser nameTextParser)
        => NameArgumentParser = nameTextParser;

    public bool IsArgumentRequired => true;

    public string Name => "icon";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = NameArgumentParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return Task.CompletedTask;
        }

        var ctx = (BuilderParentParsingContext<IChoiceOptionPrototype>)parentParsingContext;
        ctx.Builders.Add(new ChoiceOptionImageNameSetter(result.Value));
        return Task.CompletedTask;
    }
}
