using FluentAssertions;
using System;

namespace IS.Reading.Parsers
{
    public static class Extensions
    {
        public static T Get<T>(this Storyboard storyboard) where T : IStoryboardItem
            => Get<T>(storyboard.Root, false);

        public static T Get<T>(this IStoryboardItem item) where T : IStoryboardItem
            => Get<T>(item.Block, false);

        public static T GetSingle<T>(this Storyboard storyboard) where T : IStoryboardItem
            => Get<T>(storyboard.Root, true);

        public static T GetSingle<T>(this IStoryboardItem item) where T : IStoryboardItem
            => Get<T>(item.Block, true);

        private static T Get<T>(StoryboardBlock block, bool single) where T : IStoryboardItem
        {
            var item = block.ForwardQueue.Dequeue();
            item.Should().BeOfType<T>();

            if (single && block.ForwardQueue.Count > 0)
            {
                var type = block.ForwardQueue.Peek().GetType();
                throw new Exception($"Era esperado que '{typeof(T).Name}' fosse o último elemento do bloco, mas existe um elemento '{type.Name}' depois dele.");
            }

            return (T)item;
        }
    }
}
