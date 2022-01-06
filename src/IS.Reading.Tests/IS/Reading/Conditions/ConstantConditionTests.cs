using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class ConstantConditionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("Texto")]
    [InlineData("")]
    [InlineData(12345)]
    [InlineData(-132)]
    public void Evaluate(object value)
    {
        var variables = A.Dummy<IVariableDictionary>();

        var sut = new ConstantCondition(value);
        var actual = sut.Evaluate(variables);

        actual.Should().Be(value);
    }

    [Theory]
    [InlineData(null, "Null")]
    [InlineData("abc", "'abc'")]
    [InlineData("", "''")]
    [InlineData(123, "123")]
    [InlineData(-234, "-234")]
    public void ToStringTest(object value, string expected)
    {
        var sut = new ConstantCondition(value);
        var actual = sut.ToString();
        actual.Should().Be(expected);
    }
}
