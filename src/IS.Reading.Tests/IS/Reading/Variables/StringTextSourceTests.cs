namespace IS.Reading.Variables;

public class StringTextSourceTests
{
    [Theory]
    [InlineData("")]
    [InlineData("alpha")]
    [InlineData("gamma")]
    public void Initialization(string value)
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());

        var sut = new StringTextSource(value);
        sut.Text.Should().Be(value);
        sut.ToString().Should().Be(value);
        sut.Build(variables).Should().Be(value);
    }
}
