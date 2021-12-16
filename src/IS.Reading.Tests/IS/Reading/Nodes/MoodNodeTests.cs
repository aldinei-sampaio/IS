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
        var sut = new MoodNode(moodType);
        sut.MoodType.Should().Be(moodType);
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

        var sut = new MoodNode(moodType);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeOfType<MoodNode>().And.BeEquivalentTo(new { MoodType = (MoodType?)null });

        invoker.ShouldContainSingle<IMoodChangeEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                MoodType = moodType,
                PersonName = personName,
                IsProtagonist = isProtagonist
            })
        );
    }

    [Fact]
    public async Task OnEnterShouldReturnMoonNodeWithPreviousMood()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MoodType = MoodType.Surprised;
        context.State.PersonName = "abc";

        var invoker = new TestInvoker(context);

        var sut = new MoodNode(MoodType.Happy);
        var ret = await sut.EnterAsync(context);
        
        ret.Should().BeOfType<MoodNode>().And.BeEquivalentTo(new { MoodType = MoodType.Surprised });

        invoker.ShouldContainSingle<IMoodChangeEvent>(
            i => i.Should().BeEquivalentTo(new
            {
                MoodType = MoodType.Happy,
                PersonName = "abc",
                IsProtagonist = false
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
        ret.Should().BeSameAs(sut);

        invoker.Count.Should().Be(0);
    }
}
