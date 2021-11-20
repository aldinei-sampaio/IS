using IS.Reading.Navigation;
using System.Collections.Generic;

namespace IS.Reading.Parsing;

public struct Block : IBlock
{
    public Queue<INode> ForwardQueue { get; } = new();

    public Stack<INode> ForwardStack { get; } = new();

    public Stack<INode> BackwardStack { get; } = new();

    public INode? Current { get; set; } = null;
}
