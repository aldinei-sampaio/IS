using FluentAssertions;
using IS.Reading.Parsers;
using IS.Reading.StoryboardItems;
using Xunit;

namespace IS.Reading.StoryboardNavigatorTests
{
    public class GenericTests
    {
        [Fact]
        public void SimpleNarration()
        {
            var sb = new Storyboard();
            sb.Root.ForwardQueue.Enqueue(new MusicItem("abertura", null));
            sb.Root.ForwardQueue.Enqueue(new BackgroundItem("floresta", null));
            var tutorial = new TutorialItem(null);
            tutorial.Block.ForwardQueue.Enqueue(new TutorialTextItem("Tutorial 1", null));
            sb.Root.ForwardQueue.Enqueue(tutorial);

            var listener = new EventListener(sb.Context);
            var data = listener.Forward(sb);
            data.Should().Be(
@"-- next --
OnMusicChange(abertura)
OnBackgroundChange(floresta)
OnTutorialOpen()
OnTutorialChange(Tutorial 1)
-- next --
OnTutorialClose()
OnBackgroundChange()
OnMusicChange()
"
            );

            data = listener.Backward(sb);
            data.Should().Be(
@"-- previous --
OnMusicChange(abertura)
OnBackgroundChange(floresta)
OnTutorialOpen()
OnTutorialChange(Tutorial 1)
-- previous --
OnTutorialClose()
OnBackgroundChange()
OnMusicChange()
"
            );
        }

        [Theory]
        [InlineData("Introduction")]
        [InlineData("Dialog")]
        public void ForwardBackward(string prefix)
        {
            var expectedForward = this.GetResourceString(prefix + "_Forward.txt");
            var expectedBackward = this.GetResourceString(prefix + "_Backward.txt");

            var sb = StoryboardParser.Parse(this.GetResourceStream(prefix + ".xml"));
            var listener = new EventListener(sb.Context);
            listener.Forward(sb).Should().Be(expectedForward);
            listener.Backward(sb).Should().Be(expectedBackward);
            listener.Forward(sb).Should().Be(expectedForward);
            listener.Backward(sb).Should().Be(expectedBackward);
        }

        //[Fact]
        //public void Simple()
        //{
        //    var target = new StoryboardNavigator();
        //    var listener = new EventListener(target.Events);

        //    target.ForwardQueue.Enqueue(new MusicItem("abertura"));
        //    target.ForwardQueue.Enqueue(new BackgroundItem("floresta"));
        //    target.ForwardQueue.Enqueue(new ProtagonistChangeStoryEvent("eva"));
        //    target.ForwardQueue.Enqueue(new TutorialItem("tutorial1"));
        //    target.ForwardQueue.Enqueue(new TutorialItem("tutorial2"));

        //    while(target.MoveNext())
        //    {
        //    }

        //    listener.Calls.Should().BeEquivalentTo(
        //        "OnMusicChange(abertura)",
        //        "OnBackgroundChange(floresta)",
        //        "OnProtagonistChange(eva)",
        //        "OnTutorialOpen",
        //        "OnTutorialChange(tutorial1)",
        //        "OnTutorialChange(tutorial2)",
        //        "OnTutorialClose"
        //    );
        //}

        //[Fact]
        //public void BackAndForth()
        //{
        //    var target = new StoryboardNavigator();
        //    var listener = new EventListener(target.Events);

        //    target.ForwardQueue.Enqueue(new MusicItem("abertura"));
        //    target.ForwardQueue.Enqueue(new BackgroundItem("floresta"));
        //    target.ForwardQueue.Enqueue(new ProtagonistChangeStoryEvent("eva"));
        //    target.ForwardQueue.Enqueue(new TutorialItem("tutorial1"));
        //    target.ForwardQueue.Enqueue(new TutorialItem("tutorial2"));

        //    target.MoveNext().Should().BeTrue();
        //    target.MoveNext().Should().BeTrue();
        //    target.MoveNext().Should().BeTrue();
        //    target.MoveNext().Should().BeTrue();
        //    target.MoveNext().Should().BeTrue();
        //    target.MovePrevious().Should().BeTrue();
        //    target.MovePrevious().Should().BeTrue();
        //    target.MovePrevious().Should().BeTrue();
        //    target.MovePrevious().Should().BeTrue();
        //    target.MovePrevious().Should().BeFalse();
        //    target.MoveNext().Should().BeTrue();
        //    target.MoveNext().Should().BeTrue();
        //    target.MoveNext().Should().BeTrue();
        //    target.MoveNext().Should().BeTrue();
        //    target.MoveNext().Should().BeFalse();

        //    listener.Calls.Should().BeEquivalentTo(
        //        "OnMusicChange(abertura)",
        //        "OnBackgroundChange(floresta)",
        //        "OnProtagonistChange(eva)",
        //        "OnTutorialOpen",
        //        "OnTutorialChange(tutorial1)",
        //        "OnTutorialChange(tutorial2)",
        //        "OnTutorialChange(tutorial1)",
        //        "OnProtagonistChange()",
        //        "OnTutorialClose",
        //        "OnBackgroundChange()",
        //        "OnMusicChange()",
        //        "OnMusicChange(abertura)",
        //        "OnBackgroundChange(floresta)",
        //        "OnProtagonistChange(eva)",
        //        "OnTutorialOpen",
        //        "OnTutorialChange(tutorial1)",
        //        "OnTutorialChange(tutorial2)",
        //        "OnTutorialClose"
        //    );
        //}

        //[Fact]
        //public void ProtagonistTalk()
        //{
        //    var target = new StoryboardNavigator();
        //    var listener = new EventListener(target.Events);

        //    target.ForwardQueue.Enqueue(new ProtagonistChangeStoryEvent("eva"));
        //    target.ForwardQueue.Enqueue(new Prota("tutorial1"));
        //    target.ForwardQueue.Enqueue(new TutorialItem("tutorial2"));
        //}
    }
}
