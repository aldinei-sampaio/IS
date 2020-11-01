using FluentAssertions;
using System;
using Xunit;

namespace IS.Reading.Parsers
{
    public static class Extensions
    {
        public static T Get<T>(this Storyboard storyboard) where T : IStoryboardItem
            => Get<T>(storyboard.Root, false);

        public static T Get<T>(this IStoryboardItem item) where T : IStoryboardItem
            => Get<T>(item.Block, false);

        public static T GetSingle<T>(this IStoryboardItem item) where T : IStoryboardItem
            => Get<T>(item.Block, true);

        private static T Get<T>(StoryboardBlock block, bool single) where T : IStoryboardItem
        {
            if (single && block.ForwardQueue.Count > 1)
                throw new Exception($"Era esperado um bloco com um único elemento, mas ele contém {block.ForwardQueue.Count} elementos.");
            var item = block.ForwardQueue.Dequeue();
            item.Should().BeOfType<T>();
            return (T)item;
        }
    }
}
