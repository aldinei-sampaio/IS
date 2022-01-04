using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class NotBetweenConditionTests
{
    [Theory]
    [InlineData(null, 1, 5, true)]
    [InlineData(null, null, 5, false)]
    [InlineData(null, 1, null, true)]
    [InlineData(2, null, null, true)]
    [InlineData(2, null, 5, false)]
    [InlineData(2, 1, null, true)]
    [InlineData(1, 1, 1, false)]
    [InlineData(1, 1, 5, false)]
    [InlineData(3, 1, 5, false)]
    [InlineData(5, 1, 5, false)]
    [InlineData(1, 2, 5, true)]
    [InlineData(6, 2, 5, true)]
    [InlineData(null, "A", "E", true)]
    [InlineData(null, null, "E", false)]
    [InlineData(null, "A", null, true)]
    [InlineData("B", null, null, true)]
    [InlineData("B", null, "E", false)]
    [InlineData("B", "A", null, true)]
    [InlineData("A", "A", "A", false)]
    [InlineData("A", "A", "E", false)]
    [InlineData("C", "A", "E", false)]
    [InlineData("E", "A", "E", false)]
    [InlineData("A", "B", "E", true)]
    [InlineData("F", "B", "E", true)]
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

        var sut = new NotBetweenCondition(operand, min, max);
        var actual = sut.Evaluate(variables);

        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void ToStringTest()
    {
        var operand = new FakeToStringCondition("Operando");
        var min = new FakeToStringCondition("Minimo");
        var max = new FakeToStringCondition("Maximo");

        var sut = new NotBetweenCondition(operand, min, max);
        var actual = sut.ToString();

        actual.Should().Be("Operando Not Between Minimo And Maximo");
    }
}
