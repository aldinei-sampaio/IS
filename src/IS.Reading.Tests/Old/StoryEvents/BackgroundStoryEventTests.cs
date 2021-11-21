using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace IS.Reading.StoryboardItems
{
    public class BackgroundStoryEventTests
    {
        //[Fact]
        //public void Execute()
        //{
        //    var context = new StoryContext();
        //    var target = new BackgroundItem("Alpha");

        //    context.Values.ContainsKey(Keys.BackgroundImage).Should().BeFalse();
        //    var rollbackEvent = target.Enter(context);
        //    rollbackEvent.Should().NotBeNull();
        //    context.Values[Keys.BackgroundImage].Should().Be("Alpha");
        //    target.Enter(context).Should().BeNull();

        //    rollbackEvent.Should().BeOfType<BackgroundItem>();
        //    rollbackEvent = rollbackEvent.Enter(context);
        //    context.Values[Keys.BackgroundImage].Should().Be(string.Empty);
        //    rollbackEvent.Enter(context);
        //    context.Values[Keys.BackgroundImage].Should().Be("Alpha");
        //}

        //[Fact]
        //public void CallEvent()
        //{
        //    var context = new StoryContext();
        //    var target = new BackgroundItem("Alpha");

        //    var check = false;
        //    context.OnBackgroundChange += (s, e) =>
        //    {
        //        e.Should().Be("Alpha");
        //        check = true;
        //    };

        //    target.Enter(context).Should().NotBeNull();
        //    check.Should().BeTrue();

        //    check = false;
        //    target.Enter(context).Should().BeNull();
        //    check.Should().BeFalse();
        //}

    }
}
