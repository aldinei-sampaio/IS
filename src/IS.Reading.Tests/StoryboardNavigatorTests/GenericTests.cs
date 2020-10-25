using FluentAssertions;
using IS.Reading.StoryboardItems;
using Xunit;

namespace IS.Reading.StoryboardNavigatorTests
{
    /*
    public class GenericTests
    {
        [Fact]
        public void Simple()
        {
            var target = new StoryboardNavigator();
            var listener = new EventListener(target.Events);

            target.ForwardQueue.Enqueue(new MusicItem("abertura"));
            target.ForwardQueue.Enqueue(new BackgroundItem("floresta"));
            target.ForwardQueue.Enqueue(new ProtagonistChangeStoryEvent("eva"));
            target.ForwardQueue.Enqueue(new TutorialItem("tutorial1"));
            target.ForwardQueue.Enqueue(new TutorialItem("tutorial2"));

            while(target.MoveNext())
            {
            }

            listener.Calls.Should().BeEquivalentTo(
                "OnMusicChange(abertura)",
                "OnBackgroundChange(floresta)",
                "OnProtagonistChange(eva)",
                "OnTutorialOpen",
                "OnTutorialChange(tutorial1)",
                "OnTutorialChange(tutorial2)",
                "OnTutorialClose"
            );
        }

        [Fact]
        public void BackAndForth()
        {
            var target = new StoryboardNavigator();
            var listener = new EventListener(target.Events);

            target.ForwardQueue.Enqueue(new MusicItem("abertura"));
            target.ForwardQueue.Enqueue(new BackgroundItem("floresta"));
            target.ForwardQueue.Enqueue(new ProtagonistChangeStoryEvent("eva"));
            target.ForwardQueue.Enqueue(new TutorialItem("tutorial1"));
            target.ForwardQueue.Enqueue(new TutorialItem("tutorial2"));

            target.MoveNext().Should().BeTrue();
            target.MoveNext().Should().BeTrue();
            target.MoveNext().Should().BeTrue();
            target.MoveNext().Should().BeTrue();
            target.MoveNext().Should().BeTrue();
            target.MovePrevious().Should().BeTrue();
            target.MovePrevious().Should().BeTrue();
            target.MovePrevious().Should().BeTrue();
            target.MovePrevious().Should().BeTrue();
            target.MovePrevious().Should().BeFalse();
            target.MoveNext().Should().BeTrue();
            target.MoveNext().Should().BeTrue();
            target.MoveNext().Should().BeTrue();
            target.MoveNext().Should().BeTrue();
            target.MoveNext().Should().BeFalse();

            listener.Calls.Should().BeEquivalentTo(
                "OnMusicChange(abertura)",
                "OnBackgroundChange(floresta)",
                "OnProtagonistChange(eva)",
                "OnTutorialOpen",
                "OnTutorialChange(tutorial1)",
                "OnTutorialChange(tutorial2)",
                "OnTutorialChange(tutorial1)",
                "OnProtagonistChange()",
                "OnTutorialClose",
                "OnBackgroundChange()",
                "OnMusicChange()",
                "OnMusicChange(abertura)",
                "OnBackgroundChange(floresta)",
                "OnProtagonistChange(eva)",
                "OnTutorialOpen",
                "OnTutorialChange(tutorial1)",
                "OnTutorialChange(tutorial2)",
                "OnTutorialClose"
            );
        }

        [Fact]
        public void ProtagonistTalk()
        {
            var target = new StoryboardNavigator();
            var listener = new EventListener(target.Events);

            target.ForwardQueue.Enqueue(new ProtagonistChangeStoryEvent("eva"));
            target.ForwardQueue.Enqueue(new Prota("tutorial1"));
            target.ForwardQueue.Enqueue(new TutorialItem("tutorial2"));
        }
    }
    */
}
