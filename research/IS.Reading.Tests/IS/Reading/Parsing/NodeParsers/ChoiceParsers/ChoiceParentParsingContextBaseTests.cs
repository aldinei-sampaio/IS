namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceParentParsingContextBaseTests
{
    private class TestClass : BuilderParentParsingContext<int>
    {
    }

    [Fact]
    public void Initialization()
    {
        var sut = new TestClass();
        sut.Builders.Should().BeEmpty();
    }
}
