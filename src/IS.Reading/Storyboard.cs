using System.Collections.Generic;

namespace IS.Reading
{
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

        public bool MoveNext()
        {
            Context.Navigation.MoveNext();
            return Move(true);
        }

        public bool MovePrevious()
        {
            Context.Navigation.MovePrevious();
            if (atEnd)
            {
                atEnd = false;
                foreach(var item in contextItems)
                    item.Enter(Context);
            }
            return Move(false);
        }

        private bool Move(bool forward)
        {
            var block = current;
            block.Current?.Leave(Context);

            for (; ; )
            {
                var item = forward ? block.MoveNext(Context) : block.MovePrevious(Context);
                if (item == null)
                {
                    if (!stack.TryPop(out block))
                    {
                        HandleContextChangingItems(forward);
                        return false;
                    }

                    if (block.Current != null)
                    {
                        block.Current.Leave(Context);
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
                            return true;

                        item.Leave(Context);
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
            for(var n = 0; n < contextItems.Count; n++)
            {
                if (contextItems[n].GetType() == type)
                {
                    contextItems[n] = item;
                    return;
                }
            }
            contextItems.Add(item);
        }

        private void HandleContextChangingItems(bool forward)
        {
            for (var n = contextItems.Count - 1; n >= 0; n--)
                contextItems[n].OnStoryboardFinish(Context);
            atEnd = forward;
        }
    }
}
