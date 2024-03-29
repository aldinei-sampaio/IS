﻿using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing;

public interface IParsingContext
{
    void LogError(IDocumentReader documentReader, string message);
    bool IsSuccess { get; }
    void RegisterDismissNode(INode node);
    IEnumerable<INode> DismissNodes { get; }
    IParsingSceneContext SceneContext { get; }
    IBlockFactory BlockFactory { get; }
}
