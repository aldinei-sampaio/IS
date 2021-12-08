namespace IS.Reading.Choices;

public class ChoiceOptionNodeTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new ChoiceOptionNode();
        sut.Key.Should().BeEmpty();
        sut.Text.Should().BeEmpty();
        sut.DisabledText.Should().BeNull();
        sut.HelpText.Should().BeNull();
        sut.ImageName.Should().BeNull();
        sut.EnabledWhen.Should().BeNull();
        sut.VisibleWhen.Should().BeNull();
    }
}
