using System.Collections.Generic;

namespace IS.Reading;

public class Storyboard
{
    public StoryboardBlock Root { get; }
    public StoryContext Context { get; }

    private StoryboardBlock current;
    private readonly Stack<StoryboardBlock> stack;
    private readonly List<IStoryboardItem> contextItems = new List<IStoryboardItem>();
    private bool atEnd = false;

    public Storyboard()
    {
        Root = new StoryboardBlock(null);
        current = Root;
        stack = new Stack<StoryboardBlock>();
        Context = new StoryContext();
    }

    public async Task<bool> MoveNextAsync()
    {
        await Context.Navigation.MoveNextAsync();
        return await MoveAsync(true);
    }

    public async Task<bool> MovePreviousAsync()
    {
        await Context.Navigation.MovePreviousAsync();
        if (atEnd)
        {
            atEnd = false;
            foreach (var item in contextItems)
                await item.EnterAsync(Context);
        }
        return await MoveAsync(false);
    }

    private async Task<bool> MoveAsync(bool forward)
    {
        var block = current;
        if (block.Current is not null)
            await block.Current.LeaveAsync(Context);

        for (; ; )
        {
            var item = forward ? await block.MoveNextAsync(Context) : await block.MovePreviousAsync(Context);

            if (item == null)
            {
                if (!stack.TryPop(out block))
                {
                    await HandleContextChangingItemsAsync(forward);
                    Context.CurrentItem = null;
                    return false;
                }

                if (block.Current != null)
                {
                    await block.Current.LeaveAsync(Context);
                    block.Current = null;
                }

                current = block;                
            }
            else
            {
                if (item.ChangesContext)
                    StoreContextChangingItem(item);

                if (item.Block == null)
                {
                    if (item.IsPause)
                    {
                        Context.CurrentItem = item;
                        return true;
                    }

                    await item.LeaveAsync(Context);
                }
                else
                {
                    stack.Push(block);
                    current = item.Block;
                    block = item.Block;
                }
            }
        }
    }

    private void StoreContextChangingItem(IStoryboardItem item)
    {
        var type = item.GetType();
        for (var n = 0; n < contextItems.Count; n++)
        {
            if (contextItems[n].GetType() == type)
            {
                contextItems[n] = item;
                return;
            }
        }
        contextItems.Add(item);
    }

    private async Task HandleContextChangingItemsAsync(bool forward)
    {
        for (var n = contextItems.Count - 1; n >= 0; n--)
            await contextItems[n].OnStoryboardFinishAsync(Context);
        atEnd = forward;
    }
}
