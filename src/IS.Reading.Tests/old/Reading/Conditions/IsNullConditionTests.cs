using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class IsNullConditionTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", false)]
    [InlineData("A", false)]
    [InlineData(0, false)]
    [InlineData(123, false)]
    [InlineData(-12345, false)]
    public void Evaluate(object operandValue, bool expectedResult)
    {
        var variables = A.Dummy<IVariableDictionary>();

        var operand = A.Fake<IConditionKeyword>(i => i.Strict());
        A.CallTo(() => operand.Evaluate(variables)).Returns(operandValue);

        var sut = new IsNullCondition(operand);
        var actual = sut.Evaluate(variables);

        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void ToStringTest()
    {
        var operand = new FakeToStringCondition("Campo");

        var sut = new IsNullCondition(operand);
        var actual = sut.ToString();

        actual.Should().Be("Campo Is Null");
    }
}
