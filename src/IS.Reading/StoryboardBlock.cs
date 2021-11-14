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
        var item = GetNext(context);
        if (item is null)
            return null;

        Current = item;
        BackwardStack.Push(await item.EnterAsync(context));

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
        var item = GetPrevious(context);
        if (item is null)
            return null;

        Current = item;
        ForwardStack.Push(await item.EnterAsync(context));

        return item;
    }
}
