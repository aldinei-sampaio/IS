namespace IS.Reading.Variables;

public class IntegerIncrementTests
{
    [Theory]
    [InlineData("a", 0)]
    [InlineData("Abc", 1)]
    [InlineData("DEF", -2)]
    [InlineData("a_b", 999999)]
    [InlineData("a_b", -999999)]
    [InlineData("alpha", int.MaxValue)]
    [InlineData("alpha", int.MinValue)]
    public void Initialization(string name, int increment)
    {
        var sut = new IntegerIncrement(name, increment);
        sut.Should().BeEquivalentTo(new
        {
            Name = name,
            Increment = increment
        });
    }

    [Theory]
    [InlineData(null, 0, 0)]
    [InlineData(0, 0, 0)]
    [InlineData(null, 1, 1)]
    [InlineData(0, 5, 5)]
    [InlineData(0, -2, -2)]
    [InlineData(10, 5, 15)]
    [InlineData(1, -1, 0)]
    [InlineData(-58, 58, 0)]
    [InlineData(36, -46, -10)]
    [InlineData(-1, 3, 2)]
    [InlineData(-1, -2, -3)]
    [InlineData(int.MaxValue, 1, int.MaxValue)]
    [InlineData(int.MinValue, -1, int.MinValue)]
    [InlineData(int.MaxValue - 10, 10000, int.MaxValue)]
    [InlineData(int.MinValue + 100, -10000, int.MinValue)]
    public void Execute(object currentValue, int increment, int expectedResult)
    {
        const string varName = "alpha";
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallTo(() => variables[varName]).Returns(currentValue);
        A.CallToSet(() => variables[varName]).To(expectedResult).DoesNothing();

        var sut = new IntegerIncrement(varName, increment);
        var reversed = sut.Execute(variables);

        reversed.Should().Be(currentValue);

        A.CallTo(() => variables[varName]).MustHaveHappenedOnceExactly();
        A.CallToSet(() => variables[varName]).To(expectedResult).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("a", 0, "a+=0")]
    [InlineData("abc", 1, "abc++")]
    [InlineData("abc", -1, "abc--")]
    [InlineData("def", 8192, "def+=8192")]
    [InlineData("def", -2, "def-=2")]
    [InlineData("a_b", 999999, "a_b+=999999")]
    [InlineData("a_b", -999999, "a_b-=999999")]
    [InlineData("alpha", int.MaxValue, "alpha+=2147483647")]
    [InlineData("alpha", int.MinValue, "alpha-=2147483648")]
    public void ToStringTest(string name, int increment, string expected)
    {
        var sut = new IntegerIncrement(name, increment);
        sut.ToString().Should().Be(expected);
    }
}
