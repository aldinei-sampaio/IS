namespace IS.Reading.Variables;

public class InterpolatedValueTests
{
    [Theory]
    [InlineData("alpha", true)]
    [InlineData("omega", false)]
    [InlineData("{", false)]
    [InlineData("}", false)]
    public void Initialization(string value, bool isVariable)
    {
        var sut = new InterpolatedValue(value, isVariable);
        sut.Value.Should().Be(value);
        sut.IsVariable.Should().Be(isVariable);
    }

    [Theory]
    [InlineData("alpha", true, "{alpha}")]
    [InlineData("omega", false, "omega")]
    [InlineData("{", false, "{{")]
    [InlineData("}", false, "}}")]
    public void ToStringWithoutVariables(string value, bool isVariable, string toString)
    {
        var sut = new InterpolatedValue(value, isVariable);
        sut.ToString().Should().Be(toString);
    }


    [Theory]
    [InlineData("alpha", true, "omega")]
    [InlineData("alpha", false, "alpha")]
    [InlineData("value", true, "12345")]
    [InlineData("value", false, "value")]
    [InlineData("undefined", true, "")]
    [InlineData("undefined", false, "undefined")]
    public void ToStringWithVariables(string value, bool isVariable, string expected)
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallTo(() => variables["alpha"]).Returns("omega");
        A.CallTo(() => variables["value"]).Returns(12345);
        A.CallTo(() => variables["undefined"]).Returns(null);

        var sut = new InterpolatedValue(value, isVariable);
        var actual = sut.ToString(variables);
        actual.Should().Be(expected);
    }
}
