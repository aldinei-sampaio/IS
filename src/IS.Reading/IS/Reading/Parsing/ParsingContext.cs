﻿using IS.Reading.Navigation;
using System.Text;
using System.Xml;

namespace IS.Reading.Parsing;

public class ParsingContext : IParsingContext
{
    private const int MaxErrorCount = 10;
    private readonly StringBuilder stringBuilder = new();
    private int errorCount = 0;

    public ParsingContext(IBlockFactory blockFactory, IParsingSceneContext parsingSceneContext)
    {
        BlockFactory = blockFactory;
        SceneContext = parsingSceneContext;
    }

    public IBlockFactory BlockFactory { get; }

    public IParsingSceneContext SceneContext { get; }

    public bool IsSuccess => errorCount == 0;

    public IEnumerable<INode> DismissNodes => dismissNodes;

    private readonly List<INode> dismissNodes = new();

    public void LogError(IDocumentReader reader, string message)
    {
        if (errorCount >= MaxErrorCount)
            return;

        errorCount++;

        if (stringBuilder.Length > 0)
            stringBuilder.AppendLine();

        stringBuilder.Append($"Linha ");
        stringBuilder.Append(reader.CurrentLineIndex);
        stringBuilder.Append(": ");

        stringBuilder.Append(message);

        if (errorCount == MaxErrorCount)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append("Número máximo de erros atingido.");
        }
    }

    public void RegisterDismissNode(INode node)
    {
        if (!dismissNodes.Contains(node))
            dismissNodes.Add(node);
    }

    public override string ToString()
        => stringBuilder.ToString();
}
