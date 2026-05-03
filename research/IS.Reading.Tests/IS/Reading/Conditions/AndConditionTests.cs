using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class AndConditionTests
{
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    [InlineData(true, true, true)]
    public void Evaluate(bool leftValue, bool rightValue, bool expectedResult)
    {
        var variables = A.Dummy<IVariableDictionary>();

        var left = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => left.Evaluate(variables)).Returns(leftValue);

        var right = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => right.Evaluate(variables)).Returns(rightValue);

        var sut = new AndCondition(left, right);
        var actual = sut.Evaluate(variables);

        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void ToStringTest()
    {
        var left = new FakeToStringCondition("Esquerda");
        var right = new FakeToStringCondition("Direita");

        var sut = new AndCondition(left, right);
        var actual = sut.ToString();

        actual.Should().Be("(Esquerda) And (Direita)");
    }
}
