using System.Collections.Generic;

namespace IS.Reading.Navigation
{
    public interface IBlock
    {
        Queue<INode> ForwardQueue { get; }
        Stack<INode> ForwardStack { get; }
        Stack<INode> BackwardStack { get; }
        INode? Current { get; set; }
    }
}
