using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MoodNodeTests
{
    public static IEnumerable<object[]> GetMoodTypes()
        => Enum.GetValues<MoodType>().Select(i => new object[] { i });

    [Theory]
    [MemberData(nameof(GetMoodTypes))]
    public void Initialization(MoodType moodType)
    {
        var childBlock = A.Dummy<IBlock>();
        var sut = new MoodNode(moodType, childBlock);
        sut.MoodType.Should().Be(moodType);
        sut.ChildBlock.Should().Be(childBlock);
    }

    [Theory]
    [InlineData(MoodType.Normal, "alpha", "beta")]
    [InlineData(MoodType.Sad, "alpha", "alpha")]
    [InlineData(MoodType.Happy, "beta", "beta")]
    public async Task OnEnterAsyncShouldRaiseEvent(MoodType moodType, string personName, string protagonist)
    {
        var isProtagonist = personName == protagonist;

        var context = A.Dummy<INavigationContext>();
        context.State.PersonName = personName;
        context.State.ProtagonistName = protagonist;

        var invoker = new TestInvoker(context);

        var childBlock = A.Dummy<IBlock>();
        var sut = new MoodNode(moodType, childBlock);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(sut);

        var @event = invoker.Single<IMoodChangeEvent>();
        @event.MoodType.Should().Be(moodType);
        @event.PersonName.Should().Be(personName);
        @event.IsProtagonist.Should().Be(isProtagonist);
    }

    [Theory]
    [MemberData(nameof(GetMoodTypes))]
    public async Task OnEnterShouldUpdateState(MoodType moodType)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MoodType = null;

        var childBlock = A.Dummy<IBlock>();
        var sut = new MoodNode(moodType, childBlock);

        await sut.EnterAsync(context);

        context.State.MoodType.Should().Be(moodType);
    }

    [Fact]
    public async Task OnEnterShouldNotRaiseEventIfMoodWasNotChanged()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MoodType = MoodType.Happy;

        var invoker = new TestInvoker(context);

        var childBlock = A.Dummy<IBlock>();
        var sut = new MoodNode(MoodType.Happy, childBlock);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(sut);

        invoker.Count.Should().Be(0);
    }
}
