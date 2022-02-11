using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class VariableConditionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("Texto")]
    [InlineData("")]
    [InlineData(12345)]
    [InlineData(-132)]
    public void Evaluate(object value)
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallTo(() => variables["VarName"]).Returns(value);

        var sut = new VariableCondition("VarName");
        var actual = sut.Evaluate(variables);

        actual.Should().Be(value);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("alpha")]
    [InlineData("beta")]
    public void ToStringTest(string varName)
    {
        var sut = new VariableCondition(varName);
        var actual = sut.ToString();
        actual.Should().Be(varName);
    }
}
