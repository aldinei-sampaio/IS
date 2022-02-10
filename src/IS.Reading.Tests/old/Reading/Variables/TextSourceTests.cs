namespace IS.Reading.Variables;

public class TextSourceTests
{
    [Theory]
    [InlineData("")]
    [InlineData("alpha")]
    [InlineData("gamma")]
    public void InitializationWithString(string value)
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());

        var sut = new TextSource(value);
        sut.Text.Should().Be(value);
        sut.Interpolator.Should().BeNull();
        sut.ToString().Should().Be(value);
        sut.ToString(variables).Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("alpha")]
    [InlineData("gamma")]
    public void InitializationWithInterpolator(string value)
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());

        var interpolator = A.Fake<IInterpolator>(i => i.Strict());
        A.CallTo(() => interpolator.ToString()).Returns(value);
        A.CallTo(() => interpolator.ToString(variables)).Returns(value);

        var sut = new TextSource(interpolator);
        sut.Text.Should().BeNull();
        sut.Interpolator.Should().BeSameAs(interpolator);
        sut.ToString().Should().Be(value);
        sut.ToString(variables).Should().Be(value);
    }

    [Fact]
    public void InitializationWithoutParameters()
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());

        var sut = new TextSource();
        sut.Text.Should().BeNull();
        sut.Interpolator.Should().BeNull();
        Assert.Throws<InvalidOperationException>(() => sut.ToString());
        Assert.Throws<InvalidOperationException>(() => sut.ToString(variables));
    }
}
