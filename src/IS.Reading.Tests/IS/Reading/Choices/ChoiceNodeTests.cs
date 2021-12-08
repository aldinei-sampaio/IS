namespace IS.Reading.Choices;

public class ChoiceNodeTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new ChoiceNode();
        sut.Default.Should().BeNull();
        sut.TimeLimit.Should().BeNull();
        sut.Options.Should().NotBeNull().And.BeEmpty();
        ((IChoiceNode)sut).Options.Should().BeSameAs(sut.Options);
    }
}
