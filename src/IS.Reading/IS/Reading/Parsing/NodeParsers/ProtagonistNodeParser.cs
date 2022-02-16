﻿using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class ProtagonistNodeParser : IProtagonistNodeParser
{
    public INameArgumentParser NameArgumentParser { get; }

    public ProtagonistNodeParser(INameArgumentParser nameArgumentParser)
        => NameArgumentParser = nameArgumentParser;

    public bool IsArgumentRequired => true;

    public string Name => "mc";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = NameArgumentParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return Task.CompletedTask;
        }

        var node = new ProtagonistNode(result.Value);
        parentParsingContext.AddNode(node);
        parsingContext.RegisterDismissNode(DismissNode);

        return Task.CompletedTask;
    }

    public INode DismissNode { get; } 
        = new ProtagonistNode(null);
}
