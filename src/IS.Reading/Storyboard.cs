using System.Collections.Generic;

namespace IS.Reading
{
    public class Storyboard
    {
        public StoryboardBlock Root { get; }
        private StoryboardBlock current;
        private readonly Stack<StoryboardBlock> forwardStack;
        private readonly Stack<StoryboardBlock> backwardStack;
        private readonly StoryContext context;

        public Storyboard()
        {
            Root = new StoryboardBlock();
            current = Root;
            forwardStack = new Stack<StoryboardBlock>();
            backwardStack = new Stack<StoryboardBlock>();
            context = new StoryContext();
        }

        public bool MoveNext()
        {
            for(; ; )
            {
                if (MoveToNextItem(current))
                    return true;

                if (!forwardStack.TryPop(out var block))
                    return false;

                current = block;
            }
        }

        private bool MoveToNextItem(StoryboardBlock block)
        {
            block.Current?.Leave(context);

            for (; ; )
            {
                var item = block.MoveNext(context);
                if (item == null)
                    return false;

                if (item.Block == null)
                {
                    if (item.IsPause)
                        return true;

                    item.Leave(context);
                }
                else
                {
                    backwardStack.Push(block);
                    current = item.Block;
                    block = item.Block;
                }
            }
        }

        public bool MovePrevious()
        {
            for (; ; )
            {
                if (MoveToPreviousItem(current))
                    return true;

                if (!backwardStack.TryPop(out var block))
                    return false;

                current = block;
            }
        }

        private bool MoveToPreviousItem(StoryboardBlock block)
        {
            block.Current?.Leave(context);

            for (; ; )
            {
                var item = block.MovePrevious(context);
                if (item == null)
                    return false;

                if (item.Block == null)
                {
                    if (item.IsPause)
                        return true;

                    item.Leave(context);
                }
                else if(item.AllowBackwardsBlockEntry)
                {
                    forwardStack.Push(block);
                    current = item.Block;
                }

                return true;
            }
        }
    }
}
