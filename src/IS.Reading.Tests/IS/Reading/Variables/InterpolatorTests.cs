namespace IS.Reading.Variables;

public class InterpolatorTests
{
    [Fact]
    public void Initialization()
    {
        var values = Enumerable.Empty<IInterpolatedValue>();
        var aproxLength = 12345;
        var sut = new Interpolator(values, aproxLength);
        sut.Values.Should().BeSameAs(values);
        sut.AproxLength.Should().Be(aproxLength);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("abc", 1, "abc")]
    [InlineData("alphabeta", 2, "alpha", "beta")]
    [InlineData("a=2", 3, "a", "=", "2")]
    public void Interpolation(string expected, int aproxLength, params string[] interpolatedValues)
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());

        var values = interpolatedValues.Select(i =>
        {
            var value = A.Fake<IInterpolatedValue>(i => i.Strict());
            A.CallTo(() => value.ToString(variables)).Returns(i);
            return value;
        });

        var sut = new Interpolator(values, aproxLength);
        sut.ToString(variables).Should().Be(expected);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("abc", 3, "abc")]
    [InlineData("valor: {value}", 14, "valor: ", "{value}")]
    [InlineData("{a}{b}{c}", 9, "{a}", "{b}", "{c}")]
    public void ToStringTest(string expected, int aproxLength, params string[] interpolatedValues)
    {
        var values = interpolatedValues.Select(i =>
        {
            var value = A.Fake<IInterpolatedValue>(i => i.Strict());
            A.CallTo(() => value.ToString()).Returns(i);
            return value;
        });

        var sut = new Interpolator(values, aproxLength);
        sut.ToString().Should().Be(expected);
    }
}
