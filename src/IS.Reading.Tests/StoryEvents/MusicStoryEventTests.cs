using FluentAssertions;
using Xunit;

namespace IS.Reading.StoryEvents
{
    public class MusicStoryEventTests
    {
        [Fact]
        public void Execute()
        {
            var context = new StoryContext();
            var target = new MusicStoryEvent("Alpha");

            context.Values.ContainsKey(MusicStoryEvent.ValueKey).Should().BeFalse();
            var rollbackEvent = target.Execute(context);
            rollbackEvent.Should().NotBeNull();
            context.Values[MusicStoryEvent.ValueKey].Should().Be("Alpha");
            target.Execute(context).Should().BeNull();

            rollbackEvent.Should().BeOfType<MusicStoryEvent>();
            rollbackEvent = rollbackEvent.Execute(context);
            context.Values[MusicStoryEvent.ValueKey].Should().Be(string.Empty);
            rollbackEvent.Execute(context);
            context.Values[MusicStoryEvent.ValueKey].Should().Be("Alpha");
        }

        [Fact]
        public void CallEvent()
        {
            var context = new StoryContext();
            var target = new MusicStoryEvent("Alpha");

            var check = false;
            context.OnMusicChange += (s, e) =>
            {
                e.Should().Be("Alpha");
                check = true;
            };

            target.Execute(context).Should().NotBeNull();
            check.Should().BeTrue();

            check = false;
            target.Execute(context).Should().BeNull();
            check.Should().BeFalse();
        }
    }
}
