using System.Collections.Generic;

namespace IS.Reading.Navigation
{
    public interface INavigationBlock
    {
        Queue<INavigationNode> ForwardQueue { get; }
        Stack<INavigationNode> ForwardStack { get; }
        Stack<INavigationNode> BackwardStack { get; }
        INavigationNode? Current { get; set; }
    }
}
