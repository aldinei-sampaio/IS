namespace IS.Reading.Choices;

public class ChoiceOptionTests
{
    [Theory]
    [InlineData("omega", "epsilon", true, null, null)]
    [InlineData("alpha", "beta", false, "gamma", "delta")]
    public void Initialization(string key, string text, bool isEnabled, string imageName, string helpText)
    {
        var sut = new ChoiceOption(key, text, isEnabled, imageName, helpText);
        sut.Key.Should().Be(key);
        sut.Text.Should().Be(text);
        sut.IsEnabled.Should().Be(isEnabled);
        sut.ImageName.Should().Be(imageName);
        sut.Tip.Should().Be(helpText);
    }
}
