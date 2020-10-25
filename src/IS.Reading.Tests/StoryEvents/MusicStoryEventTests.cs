using FluentAssertions;
using Xunit;

namespace IS.Reading.StoryboardItems
{
    public class MusicStoryEventTests
    {
        //[Fact]
        //public void Execute()
        //{
        //    var context = new StoryContext();
        //    var target = new MusicItem("Alpha");

        //    context.Values.ContainsKey(Keys.Music).Should().BeFalse();
        //    var rollbackEvent = target.Enter(context);
        //    rollbackEvent.Should().NotBeNull();
        //    context.Values[Keys.Music].Should().Be("Alpha");
        //    target.Enter(context).Should().BeNull();

        //    rollbackEvent.Should().BeOfType<MusicItem>();
        //    rollbackEvent = rollbackEvent.Enter(context);
        //    context.Values[Keys.Music].Should().Be(string.Empty);
        //    rollbackEvent.Enter(context);
        //    context.Values[Keys.Music].Should().Be("Alpha");
        //}

        //[Fact]
        //public void CallEvent()
        //{
        //    var context = new StoryContext();
        //    var target = new MusicItem("Alpha");

        //    var check = false;
        //    context.OnMusicChange += (s, e) =>
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
