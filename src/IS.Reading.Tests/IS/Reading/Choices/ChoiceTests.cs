namespace IS.Reading.Choices;

public class ChoiceTests
{
    [Fact]
    public void Initialization()
    {
        var options = A.Dummy<IEnumerable<IChoiceOption>>();
        var timeLimit = TimeSpan.FromSeconds(1);
        var @default = "cruzcredo";
        var key = "alpha";

        var sut = new Choice(key, options, timeLimit, @default);

        sut.Key.Should().Be(key);
        sut.Options.Should().BeEquivalentTo(options);
        sut.TimeLimit.Should().Be(timeLimit);
        sut.Default.Should().BeEquivalentTo(@default);
    }

    [Fact]
    public void InitializationWithNullArguments()
    {
        var options = A.Dummy<IEnumerable<IChoiceOption>>();
        var key = "gamma";

        var sut = new Choice(key, options, null, null);

        sut.Key.Should().Be(key);
        sut.Options.Should().BeEquivalentTo(options);
        sut.TimeLimit.Should().BeNull();
        sut.Default.Should().BeNull();
    }
}
