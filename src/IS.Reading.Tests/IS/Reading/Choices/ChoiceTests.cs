namespace IS.Reading.Choices;

public class ChoiceTests
{
    [Fact]
    public void Initialization()
    {
        var options = A.Dummy<IEnumerable<IChoiceOption>>();
        var timeLimit = TimeSpan.FromSeconds(1);
        var @default = "cruzcredo";

        var sut = new Choice(options, timeLimit, @default);

        sut.Options.Should().BeEquivalentTo(options);
        sut.TimeLimit.Should().Be(timeLimit);
        sut.Default.Should().BeEquivalentTo(@default);
    }

    [Fact]
    public void InitializationWithNullArguments()
    {
        var options = A.Dummy<IEnumerable<IChoiceOption>>();

        var sut = new Choice(options, null, null);

        sut.Options.Should().BeEquivalentTo(options);
        sut.TimeLimit.Should().BeNull();
        sut.Default.Should().BeNull();
    }
}
