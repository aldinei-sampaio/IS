using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace IS.Reading.StoryboardItems
{
    public class NarrationStoryEventTests
    {
        [Fact]
        public void Execute()
        {
            var context = new StoryContext();
            var target = new NarrationStoryEvent("Zulu");

            var events = new List<string>();

            context.OnNarrationOpen += (s, e) => events.Add("Open");
            context.OnNarrationChange += (s, e) => events.Add("Change: " + e);
            context.OnNarrationClose += (s, e) => events.Add("Close");

            target.Execute(context);
            context.ClosePending();

            events.Should().BeEquivalentTo("Open", "Change: Zulu", "Close");
        }
    }
}
