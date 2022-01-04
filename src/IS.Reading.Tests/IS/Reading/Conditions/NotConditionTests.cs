using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class NotConditionTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Evaluate(bool value)
    {
        var variables = A.Dummy<IVariableDictionary>();

        var condition = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => condition.Evaluate(variables)).Returns(value);

        var sut = new NotCondition(condition);
        var actual = sut.Evaluate(variables);

        actual.Should().Be(!value);
    }

    [Fact]
    public void ToStringTest()
    {
        var condition = new FakeToStringCondition("Condição");

        var sut = new NotCondition(condition);
        var actual = sut.ToString();

        actual.Should().Be("Not (Condição)");
    }
}
