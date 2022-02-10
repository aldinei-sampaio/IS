namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceParentParsingContextTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new ChoiceParentParsingContext();
        sut.Choice.Should().NotBeNull();
    }
}
