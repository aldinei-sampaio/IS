﻿using IS.Reading.Variables;

namespace IS.Reading.Conditions;

public class EqualOrLowerThanConditionTests
{
    [Theory]
    [InlineData(null, 1, true)]
    [InlineData(null, null, true)]
    [InlineData(1, null, false)]
    [InlineData(1, 1, true)]
    [InlineData(5, 3, false)]
    [InlineData(1, 2, true)]
    [InlineData(null, "1", true)]
    [InlineData("1", null, false)]
    [InlineData("1", "1", true)]
    [InlineData("5", "3", false)]
    [InlineData("1", "2", true)]
    [InlineData("2", 2, false)]
    [InlineData(2, "5", false)]
    public void Evaluate(object leftValue, object rightValue, bool expectedResult)
    {
        var variables = A.Dummy<IVariableDictionary>();

        var left = A.Fake<IConditionKeyword>(i => i.Strict());
        A.CallTo(() => left.Evaluate(variables)).Returns(leftValue);

        var right = A.Fake<IConditionKeyword>(i => i.Strict());
        A.CallTo(() => right.Evaluate(variables)).Returns(rightValue);

        var sut = new EqualOrLowerThanCondition(left, right);
        var actual = sut.Evaluate(variables);

        actual.Should().Be(expectedResult);
    }

    [Fact]
    public void ToStringTest()
    {
        var left = new FakeToStringCondition("Valor1");
        var right = new FakeToStringCondition("Valor2");

        var sut = new EqualOrLowerThanCondition(left, right);
        var actual = sut.ToString();

        actual.Should().Be("Valor1 <= Valor2");
    }
}
