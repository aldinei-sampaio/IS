using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class IsNotNullConditionTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", true)]
    [InlineData("A", true)]
    [InlineData(0, true)]
    [InlineData(123, true)]
    [InlineData(-12345, true)]
    public void Evaluate(object operandValue, bool expectedResult)
    {
        var variables = A.Dummy<IVariableDictionary>();

        var operand = A.Fake<IConditionKeyword>(i => i.Strict());
        A.CallTo(() => operand.Evaluate(variables)).Returns(operandValue);

        var sut = new IsNotNullCondition(operand);
        var actual = sut.Evaluate(variables);

        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void ToStringTest()
    {
        var operand = new FakeToStringCondition("Campo");

        var sut = new IsNotNullCondition(operand);
        var actual = sut.ToString();

        actual.Should().Be("Campo Is Not Null");
    }
}
