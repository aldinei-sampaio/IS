using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class OrConditionTests
{
    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    [InlineData(true, true, true)]
    public void Evaluate(bool leftValue, bool rightValue, bool expectedResult)
    {
        var variables = A.Dummy<IVariableDictionary>();

        var left = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => left.Evaluate(variables)).Returns(leftValue);

        var right = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => right.Evaluate(variables)).Returns(rightValue);

        var sut = new OrCondition(left, right);
        var actual = sut.Evaluate(variables);

        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void ToStringTest()
    {
        var left = new FakeToStringCondition("Esquerda");
        var right = new FakeToStringCondition("Direita");

        var sut = new OrCondition(left, right);
        var actual = sut.ToString();

        actual.Should().Be("(Esquerda) Or (Direita)");
    }
}
