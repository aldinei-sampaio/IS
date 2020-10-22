using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace IS.Reading.StoryEvents
{
    public class BackgroundStoryEventTests
    {
        [Fact]
        public void Execute()
        {
            var context = new StoryContext();
            var target = new BackgroundStoryEvent("Alpha");

            context.Values.ContainsKey(BackgroundStoryEvent.ValueKey).Should().BeFalse();
            var rollbackEvent = target.Execute(context);
            rollbackEvent.Should().NotBeNull();
            context.Values[BackgroundStoryEvent.ValueKey].Should().Be("Alpha");
            target.Execute(context).Should().BeNull();

            rollbackEvent.Should().BeOfType<BackgroundStoryEvent>();
            rollbackEvent = rollbackEvent.Execute(context);
            context.Values[BackgroundStoryEvent.ValueKey].Should().Be(string.Empty);
            rollbackEvent.Execute(context);
            context.Values[BackgroundStoryEvent.ValueKey].Should().Be("Alpha");
        }

        [Fact]
        public void CallEvent()
        {
            var context = new StoryContext();
            var target = new BackgroundStoryEvent("Alpha");

            var check = false;
            context.OnBackgroundChange += (s, e) =>
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
