using FluentAssertions;

namespace IS.Reading.Parsers
{
    public static class Extensions
    {
        public static T Get<T>(this Storyboard storyboard) where T : IStoryboardItem
            => Get<T>(storyboard.Root);

        public static T Get<T>(this IStoryboardItem item) where T : IStoryboardItem
            => Get<T>(item.Block);

        private static T Get<T>(StoryboardBlock block) where T : IStoryboardItem
        {
            var item = block.ForwardQueue.Dequeue();
            item.Should().BeOfType<T>();
            return (T)item;
        }
    }
}
