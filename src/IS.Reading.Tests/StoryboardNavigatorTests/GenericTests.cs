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

        [Fact]
        public void Introduction()
        {
            var sb = StoryboardParser.Load(Resource.Introduction);
            var listener = new EventListener(sb.Context);
            listener.Forward(sb).Should().Be(Resource.Introduction_Forward);
            listener.Backward(sb).Should().Be(Resource.Introduction_Backward);
            listener.Forward(sb).Should().Be(Resource.Introduction_Forward);
            listener.Backward(sb).Should().Be(Resource.Introduction_Backward);
        }

        [Fact]
        public void Dialog()
        {
            var sb = StoryboardParser.Load(Resource.Dialog);
            var listener = new EventListener(sb.Context);
            listener.Forward(sb).Should().Be(Resource.Dialog_Forward);
            listener.Backward(sb).Should().Be(Resource.Dialog_Backward);
            listener.Forward(sb).Should().Be(Resource.Dialog_Forward);
            listener.Backward(sb).Should().Be(Resource.Dialog_Backward);
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
