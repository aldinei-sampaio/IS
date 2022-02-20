namespace IS.Reading.State;

public class NavigationStateTests
{
    private readonly IBlockStateDictionary blockStateDictionary;
    private readonly NavigationState sut;

    public NavigationStateTests()
    {
        blockStateDictionary = A.Fake<IBlockStateDictionary>(i => i.Strict());
        sut = new(blockStateDictionary);
    }

    [Fact]
    public void Initialization()
    {
        sut.Background.Should().NotBeNull();
        sut.Background.Should().BeSameAs(BackgroundState.Empty);        
        sut.MainCharacterName.Should().BeNull();
        sut.MoodType.Should().BeNull();
        sut.PersonName.Should().BeNull();
        sut.MusicName.Should().BeNull();
        sut.BlockStates.Should().BeSameAs(blockStateDictionary);
        sut.CurrentBlockId.Should().BeNull();
        sut.CurrentIteration.Should().Be(0);
        sut.WaitingFor.Should().BeNull();
    }

    [Fact]
    public void ReadWriteProperties()
    {
        sut.MainCharacterName = "agenor";
        sut.MoodType = MoodType.Happy;
        sut.PersonName = "person";
        sut.MusicName = "music";
        sut.CurrentBlockId = 957;
        sut.CurrentIteration = 1;
        sut.WaitingFor = "qualquercoisa";

        sut.MainCharacterName.Should().Be("agenor");
        sut.MoodType.Should().Be(MoodType.Happy);
        sut.PersonName.Should().Be("person");
        sut.MusicName.Should().Be("music");
        sut.CurrentBlockId.Should().Be(957);
        sut.CurrentIteration.Should().Be(1);
        sut.WaitingFor.Should().Be("qualquercoisa");
    }
}
