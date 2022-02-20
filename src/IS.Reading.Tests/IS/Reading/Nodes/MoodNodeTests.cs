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
        var sut = new MoodNode(moodType);
        sut.MoodType.Should().Be(moodType);
    }

    [Theory]
    [InlineData(MoodType.Surprised, "alpha", "beta")]
    [InlineData(MoodType.Sad, "alpha", "alpha")]
    [InlineData(MoodType.Happy, "beta", "beta")]
    public async Task OnEnterAsyncShouldRaiseEvent(MoodType moodType, string personName, string mainCharacterName)
    {
        var isMainCharacter = personName == mainCharacterName;

        var context = A.Dummy<INavigationContext>();
        context.State.PersonName = personName;
        context.State.MainCharacterName = mainCharacterName;
        context.State.MoodType = null;

        var invoker = new TestInvoker(context);

        var sut = new MoodNode(moodType);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeNull();

        invoker.ShouldContainSingle<IMoodChangeEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                MoodType = moodType,
                PersonName = personName,
                IsMainCharacter = isMainCharacter
            })
        );
    }

    [Fact]
    public async Task ShouldNotRaiseEventForNomalMoodWhenContextMoodIsNull()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MoodType = null;

        var invoker = new TestInvoker(context);

        var sut = new MoodNode(MoodType.Normal);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeNull();

        invoker.Count.Should().Be(0);
    }

    [Fact]
    public async Task OnEnterShouldReturnPreviousMood()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MoodType = MoodType.Surprised;
        context.State.PersonName = "abc";

        var invoker = new TestInvoker(context);

        var sut = new MoodNode(MoodType.Happy);
        var ret = await sut.EnterAsync(context);
        
        ret.Should().Be(MoodType.Surprised);

        invoker.ShouldContainSingle<IMoodChangeEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                MoodType = MoodType.Happy,
                PersonName = "abc",
                IsMainCharacter = false
            })
        );
    }

    [Theory]
    [MemberData(nameof(GetMoodTypes))]
    public async Task OnEnterShouldUpdateState(MoodType moodType)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MoodType = null;

        var sut = new MoodNode(moodType);

        await sut.EnterAsync(context);

        context.State.MoodType.Should().Be(moodType);
    }

    [Fact]
    public async Task OnEnterShouldNotRaiseEventIfMoodWasNotChanged()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MoodType = MoodType.Happy;

        var invoker = new TestInvoker(context);

        var sut = new MoodNode(MoodType.Happy);

        var ret = await sut.EnterAsync(context);
        ret.Should().Be(MoodType.Happy);

        invoker.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(MoodType.Surprised)]
    [InlineData(MoodType.Sad)]
    [InlineData(MoodType.Angry)]
    [InlineData(MoodType.Normal)]
    public async Task ShouldRaiseEventWithStateArg(MoodType moodType)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.PersonName = "alpha";
        context.State.MainCharacterName = "alpha";
        context.State.MoodType = MoodType.Happy;

        var invoker = new TestInvoker(context);

        var sut = new MoodNode(MoodType.Happy);

        await sut.EnterAsync(context, moodType);

        invoker.ShouldContainSingle<IMoodChangeEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                MoodType = moodType,
                PersonName = "alpha",
                IsMainCharacter = true
            })
        );
    }
}
