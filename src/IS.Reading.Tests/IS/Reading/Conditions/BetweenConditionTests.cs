using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class BetweenConditionTests
{
    [Theory]
    [InlineData(null, 1, 5, false)]
    [InlineData(null, null, 5, true)]
    [InlineData(null, 1, null, false)]
    [InlineData(2, null, null, false)]
    [InlineData(2, null, 5, true)]
    [InlineData(2, 1, null, false)]
    [InlineData(1, 1, 1, true)]
    [InlineData(1, 1, 5, true)]
    [InlineData(3, 1, 5, true)]
    [InlineData(5, 1, 5, true)]
    [InlineData(1, 2, 5, false)]
    [InlineData(6, 2, 5, false)]
    [InlineData(null, "A", "E", false)]
    [InlineData(null, null, "E", true)]
    [InlineData(null, "A", null, false)]
    [InlineData("B", null, null, false)]
    [InlineData("B", null, "E", true)]
    [InlineData("B", "A", null, false)]
    [InlineData("A", "A", "A", true)]
    [InlineData("A", "A", "E", true)]
    [InlineData("C", "A", "E", true)]
    [InlineData("E", "A", "E", true)]
    [InlineData("A", "B", "E", false)]
    [InlineData("F", "B", "E", false)]
    [InlineData("2", 2, 5, false)]
    [InlineData("2", "2", 5, false)]
    [InlineData("2", 2, "5", false)]
    [InlineData(2, "2", 5, false)]
    [InlineData(2, "2", "5", false)]
    public void Evaluate(object operandValue, object minValue, object maxValue, bool expectedResult)
    {
        var variables = A.Dummy<IVariableDictionary>();

        var operand = A.Fake<IConditionKeyword>(i => i.Strict());
        A.CallTo(() => operand.Evaluate(variables)).Returns(operandValue);

        var min = A.Fake<IConditionKeyword>(i => i.Strict());
        A.CallTo(() => min.Evaluate(variables)).Returns(minValue);

        var max = A.Fake<IConditionKeyword>(i => i.Strict());
        A.CallTo(() => max.Evaluate(variables)).Returns(maxValue);

        var sut = new BetweenCondition(operand, min, max);
        var actual = sut.Evaluate(variables);

        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void ToStringTest()
    {
        var operand = new FakeToStringCondition("Operando");
        var min = new FakeToStringCondition("Minimo");
        var max = new FakeToStringCondition("Maximo");

        var sut = new BetweenCondition(operand, min, max);
        var actual = sut.ToString();

        actual.Should().Be("Operando Between Minimo And Maximo");
    }
}
