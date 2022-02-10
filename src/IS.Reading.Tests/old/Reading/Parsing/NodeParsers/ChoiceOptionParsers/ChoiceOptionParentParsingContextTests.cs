namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionParentParsingContextTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new ChoiceOptionParentParsingContext();
        sut.Option.Should().NotBeNull();
    }
}
