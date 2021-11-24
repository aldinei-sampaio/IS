﻿using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class BackgroundColorNode : INode
{
    public string Color { get; }

    public BackgroundColorNode(string color, ICondition? condition)
        => (Color, When) = (color, condition);

    public ICondition? When { get; }

    public ICondition? While => null;

    public IBlock? ChildBlock => null;

    public Task<INode> EnterAsync(INavigationContext context)
    {
        throw new NotImplementedException();
    }

    public Task LeaveAsync(INavigationContext context)
    {
        throw new NotImplementedException();
    }
}
