namespace IS.Reading.Variables;

public class StringTextSourceTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("alpha", "alpha")]
    [InlineData("gamma", "gamma")]
    [InlineData("{abc}", "{{abc}}")]
    public void Initialization(string value, string toStringResult)
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());

        var sut = new StringTextSource(value);
        sut.Text.Should().Be(value);
        sut.ToString().Should().Be(toStringResult);
        sut.Build(variables).Should().Be(value);
    }
}
