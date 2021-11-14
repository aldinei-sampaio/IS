using System.Collections.Generic;

namespace IS.Reading;

public class StoryboardBlock
{
    public StoryboardBlock(IStoryboardItem? parent) => Parent = parent;

    public IStoryboardItem? Parent { get; }
    public Queue<IStoryboardItem> ForwardQueue { get; } = new Queue<IStoryboardItem>();
    public Stack<IStoryboardItem> ForwardStack { get; } = new Stack<IStoryboardItem>();
    public Stack<IStoryboardItem> BackwardStack { get; } = new Stack<IStoryboardItem>();
    public IStoryboardItem? Current { get; set; }
    private IStoryboardItem? backCurrent;
    private IStoryboardItem? forwardCurrent;

    private IStoryboardItem? GetNext()
    {
        if (ForwardStack.TryPop(out var stackItem))
            return stackItem;

        if (ForwardQueue.TryDequeue(out var queueItem))
            return queueItem;

        return null;
    }

    private IStoryboardItem? GetNext(StoryContext context)
    {
        for (; ; )
        {
            var item = GetNext();
            if (item == null)
                return null;

            if (item.Condition == null || item.Condition.Evaluate(context))
                return item;
        }
    }

    public async Task<IStoryboardItem?> MoveNextAsync(StoryContext context)
    {
        if (forwardCurrent is not null)
        {
            if (Current is not null)
                BackwardStack.Push(Current);
            forwardCurrent = null;
        }

        if (backCurrent is not null)
            BackwardStack.Push(backCurrent);

        var item = GetNext(context);
        if (item is null)
            return null;

        Current = item;
        backCurrent = await item.EnterAsync(context);

        return item;
    }

    private IStoryboardItem? GetPrevious(StoryContext context)
    {
        for (; ; )
        {
            if (!BackwardStack.TryPop(out var item))
                return null;

            if (item.Condition == null || item.Condition.Evaluate(context))
                return item;
        }
    }

    public async Task<IStoryboardItem?> MovePreviousAsync(StoryContext context)
    {
        if (backCurrent is not null)
        {
            if (Current is not null)
                ForwardStack.Push(Current);
            backCurrent = null;
        }

        if (forwardCurrent is not null)
            ForwardStack.Push(forwardCurrent);

        var item = GetPrevious(context);
        if (item is null)
            return null;

        Current = item;

        forwardCurrent = await item.EnterAsync(context);

        return item;
    }
}
