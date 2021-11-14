using FluentAssertions;
using IS.Reading.Parsers;
using IS.Reading.StoryboardItems;
using System.Collections.Generic;
using Xunit;

namespace IS.Reading.StoryboardNavigatorTests;

public class GenericTests
{
    [Fact]
    public async Task SimpleNarration()
    {
        var sb = new Storyboard();
        sb.Root.ForwardQueue.Enqueue(new MusicItem("abertura", null));
        sb.Root.ForwardQueue.Enqueue(new BackgroundItem("floresta", null));
        var tutorial = new TutorialItem(null);
        tutorial.Block.ForwardQueue.Enqueue(new TutorialTextItem("Tutorial 1", null));
        sb.Root.ForwardQueue.Enqueue(tutorial);

        var listener = new EventListener(sb.Context);
        var data = await listener.ForwardAsync(sb);
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

        data = await listener.BackwardAsync(sb);
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
    public void PromptAndEmotion()
    {
        var sb = StoryboardParser.Parse(this.GetResourceStream("PromptAndEmotion.xml"));
        var prot = sb.GetSingle<ProtagonistItem>();
        var mood = prot.GetSingle<ProtagonistMoodItem>();
        var prompt = mood.GetSingle<PromptItem>();
        prompt.GetSingle<ProtagonistThoughtItem>().GetSingle<ProtagonistThoughtTextItem>().Text.Should().Be("...");
    }

    [Theory]
    [InlineData("Introduction")]
    [InlineData("Dialog")]
    [InlineData("Prompt")]
    [InlineData("PromptAndEmotion")]
    [InlineData("Bump")]
    [InlineData("Do")]
    public async Task ForwardBackward(string prefix)
    {
        var expectedForward = this.GetResourceString(prefix + "_Forward.txt");
        var expectedBackward = this.GetResourceString(prefix + "_Backward.txt");

        var sb = StoryboardParser.Parse(this.GetResourceStream(prefix + ".xml"));
        var listener = new EventListener(sb.Context);
        (await listener.ForwardAsync(sb)).Should().Be(expectedForward);
        (await listener.BackwardAsync(sb)).Should().Be(expectedBackward);
        (await listener.ForwardAsync(sb)).Should().Be(expectedForward);
        (await listener.BackwardAsync(sb)).Should().Be(expectedBackward);
    }

    [Fact]
    public async Task Simple()
    {
        var sb = new Storyboard();

        sb.Root.ForwardQueue.Enqueue(new MusicItem("abertura", null));
        sb.Root.ForwardQueue.Enqueue(new BackgroundItem("floresta", null));
        sb.Root.ForwardQueue.Enqueue(new ProtagonistChangeItem("eva", null));
        
        var t1 = new TutorialItem(null);
        t1.Block.ForwardQueue.Enqueue(new TutorialTextItem("tutorial1", null));
        t1.Block.ForwardQueue.Enqueue(new TutorialTextItem("tutorial2", null));
        sb.Root.ForwardQueue.Enqueue(t1);

        var listener = new EventListener(sb.Context);
        var data = await listener.ForwardAsync(sb);

        data.Should().BeEquivalentTo(
@"-- next --
OnMusicChange(abertura)
OnBackgroundChange(floresta)
OnProtagonistChange(eva)
OnTutorialOpen()
OnTutorialChange(tutorial1)
-- next --
OnTutorialChange(tutorial2)
-- next --
OnTutorialClose()
OnProtagonistChange()
OnBackgroundChange()
OnMusicChange()
");
    }

    [Fact]
    public async Task BackAndForth()
    {
        var target = new Storyboard();
        var listener = new EventListener(target.Context);

        target.Root.ForwardQueue.Enqueue(new MusicItem("abertura", null));
        target.Root.ForwardQueue.Enqueue(new BackgroundItem("floresta", null));
        target.Root.ForwardQueue.Enqueue(new ProtagonistChangeItem("eva", null));
        var t1 = new TutorialItem(null);
        t1.Block.ForwardQueue.Enqueue(new TutorialTextItem("tutorial1", null));
        t1.Block.ForwardQueue.Enqueue(new TutorialTextItem("tutorial2", null));
        target.Root.ForwardQueue.Enqueue(t1);

        (await target.MoveNextAsync()).Should().BeTrue();
        (await target.MoveNextAsync()).Should().BeTrue();
        (await target.MovePreviousAsync()).Should().BeTrue();
        (await target.MovePreviousAsync()).Should().BeFalse();
        (await target.MoveNextAsync()).Should().BeTrue();
        (await target.MoveNextAsync()).Should().BeTrue();
        (await target.MoveNextAsync()).Should().BeFalse();

        listener.ToString().Should().Be(
@"OnMusicChange(abertura)
OnBackgroundChange(floresta)
OnProtagonistChange(eva)
OnTutorialOpen
OnTutorialChange(tutorial1)
OnTutorialChange(tutorial2)
OnTutorialChange(tutorial1)
OnProtagonistChange()
OnTutorialClose
OnBackgroundChange()
OnMusicChange()
OnMusicChange(abertura)
OnBackgroundChange(floresta)
OnProtagonistChange(eva)
OnTutorialOpen
OnTutorialChange(tutorial1)
OnTutorialChange(tutorial2)
OnTutorialClose"
        );
    }

    //[Fact]
    //public void ProtagonistTalk()
    //{
    //    var target = new StoryboardNavigator();
    //    var listener = new EventListener(target.Events);

    //    target.ForwardQueue.Enqueue(new ProtagonistChangeStoryEvent("eva"));
    //    target.ForwardQueue.Enqueue(new Prota("tutorial1"));
    //    target.ForwardQueue.Enqueue(new TutorialItem("tutorial2"));
    //}

    [Fact]
    public void NavTest()
    {
        var nav = new Navigator();
        nav.InitialQueue.Enqueue(new NavItem("t1"));
        nav.InitialQueue.Enqueue(new NavItem("t2"));
        nav.InitialQueue.Enqueue(new NavItem("t3"));

        nav.MoveNext().ToString().Should().Be("t1");
        nav.MoveNext().ToString().Should().Be("t2");
        nav.MovePrevious().ToString().Should().Be("t1(R)");
        nav.MoveNext().ToString().Should().Be("t2");
        nav.MoveNext().ToString().Should().Be("t3");
        nav.MovePrevious().ToString().Should().Be("t2(R)");
        nav.MovePrevious().ToString().Should().Be("t1(R)");
        nav.MovePrevious().Should().BeNull();
        nav.MoveNext().ToString().Should().Be("t1");
        nav.MoveNext().ToString().Should().Be("t2");
        nav.MoveNext().ToString().Should().Be("t3");
        nav.MoveNext().Should().BeNull();
        nav.MovePrevious().ToString().Should().Be("t3(R)");
        nav.MovePrevious().ToString().Should().Be("t2(R)");
        nav.MovePrevious().ToString().Should().Be("t1(R)");
    }

    public class NavItem
    {
        public string Data { get; }
        public NavItem(string data)
        {
            Data = data;
        }
        public bool Reversed { get; private set; }
        public NavItem OnEnter() => new NavItem(Data) { Reversed = !this.Reversed };
        public NavItem OnLeave() => this;
        public override string ToString() => this.Reversed ? Data + "(R)" : Data;
    }

    public class Navigator
    {
        public Queue<NavItem> InitialQueue { get; } = new();
        public Stack<NavItem> ForwardStack { get; } = new();
        public Stack<NavItem> BackwardStack { get; } = new();

        private NavItem forwardCurrent;
        private NavItem backwardCurrent;

        public NavItem MoveNext()
        {
            if (backwardCurrent is not null)
                BackwardStack.Push(backwardCurrent);

            if (ForwardStack.TryPop(out var stackItem))
            {
                forwardCurrent = stackItem;
                backwardCurrent = stackItem.OnEnter();
                return stackItem;
            }

            if (InitialQueue.TryDequeue(out var item))
            {
                forwardCurrent = item;
                backwardCurrent = item.OnEnter();
                return item;
            }
            return null;
        }

        public NavItem MovePrevious()
        {
            if (forwardCurrent is not null)
                ForwardStack.Push(forwardCurrent);

            if (BackwardStack.TryPop(out var item))
            {
                backwardCurrent = item;
                forwardCurrent = item.OnEnter();
                return item;
            }

            return null;
        }
    }
}
