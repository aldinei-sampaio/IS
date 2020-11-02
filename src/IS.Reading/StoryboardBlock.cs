using System.Collections.Generic;

namespace IS.Reading
{
    public class StoryboardBlock
    {
        public StoryboardBlock(IStoryboardItem? parent) => Parent = parent;        

        public IStoryboardItem? Parent { get; }
        public Queue<IStoryboardItem> ForwardQueue { get; } = new Queue<IStoryboardItem>();
        public Stack<IStoryboardItem> ForwardStack { get; } = new Stack<IStoryboardItem>();
        public Stack<IStoryboardItem> BackwardStack { get; } = new Stack<IStoryboardItem>();
        public IStoryboardItem? Current { get; set; }

        public IStoryboardItem? MoveNext(StoryContext context)
        {
            IStoryboardItem item;
            do
            {
                if (!ForwardStack.TryPop(out item) && !ForwardQueue.TryDequeue(out item))
                    return null;
            }
            while (item.Condition != null && !item.Condition.Evaluate(context));

            Current = item;
            BackwardStack.Push(item.Enter(context));
            
            return item;
        }

        public IStoryboardItem? MovePrevious(StoryContext context)
        {
            IStoryboardItem item;
            do
            {
                if (!BackwardStack.TryPop(out item))
                    return null;
            }
            while (item.Condition != null && !item.Condition.Evaluate(context));

            Current = item;
            ForwardStack.Push(item.Enter(context));

            return item;
        }
    }
}
