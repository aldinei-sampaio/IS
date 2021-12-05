using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MusicNodeTests
{
    [Fact]
    public void Initialization()
    {
        var musicName = "never_look_back";
        var when = A.Dummy<ICondition>();
        var sut = new MusicNode(musicName, when);
        sut.MusicName.Should().Be(musicName);
        sut.When.Should().Be(when);
    }

    [Fact]
    public async Task OnEnterAsyncShouldReturnReversedNode()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "enigma";

        var sut = new MusicNode("free_mind", null);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeOfType<MusicNode>()
            .Which.MusicName.Should().Be("enigma");
    }

    [Fact]
    public async Task OnEnterAsyncShouldRaiseEvent()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = null;

        var invoker = new TestInvoker(context);

        var sut = new MusicNode("goodbye", null);
        await sut.EnterAsync(context);

        var @event = invoker.Single<IMusicChangeEvent>();
        @event.MusicName.Should().Be("goodbye");
    }

    [Fact]
    public async Task OnEnterAsyncShouldUpdateState()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "sad_story";

        var sut = new MusicNode("open_sky", null);
        await sut.EnterAsync(context);

        context.State.MusicName.Should().Be("open_sky");
    }

    [Fact]
    public async Task OnEnterAsyncShouldNotRaiseEventIfMusicWasNotChanged()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "crime_fighter";

        var invoker = new TestInvoker(context);

        var sut = new MusicNode("crime_fighter", null);
        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(sut);

        invoker.Count.Should().Be(0);
    }
}
