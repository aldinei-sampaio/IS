using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class NotInConditionTests
{
    [Theory]
    [InlineData(true, null, 1)]
    [InlineData(true, null, 1, 2)]
    [InlineData(false, null, new object[] { null })]
    [InlineData(false, null, 1, null)]
    [InlineData(false, 1, 1)]
    [InlineData(false, 2, 1, 2)]
    [InlineData(false, 2, null, 1, 2)]
    [InlineData(true, 4, 1, 2)]
    [InlineData(true, 4)]
    [InlineData(false, "1", "1")]
    [InlineData(false, "2", "1", "2")]
    [InlineData(false, "2", null, "1", "2")]
    [InlineData(true, "4", "1", "2")]
    [InlineData(true, "4")]
    [InlineData(false, "1", 1, "1")]
    [InlineData(false, "1", "1", 1)]
    [InlineData(true, "1", "2", 1)]
    [InlineData(true, "2", "1", 2)]
    [InlineData(false, 1, 1, "1")]
    [InlineData(true, 1, "1", 2)]
    [InlineData(true, 2, "2", 1)]
    public void Evaluate(bool expectedResult, object operandValue, params object[] keywordValues)
    {
        var variables = A.Dummy<IVariableDictionary>();

        var operand = A.Fake<IConditionKeyword>(i => i.Strict());
        A.CallTo(() => operand.Evaluate(variables)).Returns(operandValue);

        var valueList = new List<IConditionKeyword>();
        foreach(var value in keywordValues)
        {
            var item = A.Fake<IConditionKeyword>(i => i.Strict());
            A.CallTo(() => item.Evaluate(variables)).Returns(value);
            valueList.Add(item);
        }

        var sut = new NotInCondition(operand, valueList);
        var actual = sut.Evaluate(variables);

        actual.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("Delta Not In ()", "Delta")]
    [InlineData("Alfa Not In (1)", "Alfa", "1")]
    [InlineData("Beta Not In (1, 2)", "Beta", "1", "2")]
    [InlineData("Gama Not In (1, 2, 3)", "Gama", "1", "2", "3")]
    [InlineData("Alfa Not In (\"1\")", "Alfa", "\"1\"")]
    [InlineData("Beta Not In (\"1\", \"2\")", "Beta", "\"1\"", "\"2\"")]
    [InlineData("Gama Not In (\"1\", \"2\", \"3\")", "Gama", "\"1\"", "\"2\"", "\"3\"")]
    [InlineData("Omega Not In (1, \"2\")", "Omega", "1", "\"2\"")]
    public void ToStringTest(string expectedExpr, string operandExpr, params string[] valuesExpr)
    {
        var operand = new FakeToStringCondition(operandExpr);
        var valueList = valuesExpr.Select(i => new FakeToStringCondition(i)).ToList();

        var sut = new NotInCondition(operand, valueList);
        var actual = sut.ToString();

        actual.Should().Be(expectedExpr);
    }
}
