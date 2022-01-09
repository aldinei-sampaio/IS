namespace IS.Reading.Variables;

public class VariableDictionaryTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new VariableDictionary();
        sut.Count.Should().Be(0);
        sut.IsSet("abc").Should().BeFalse();
        sut["abc"].Should().BeNull();
    }

    [Theory]
    [InlineData("abc")]
    [InlineData(0)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    [InlineData(56)]
    [InlineData(-112)]
    public void SetOneValue(object value)
    {
        var sut = new VariableDictionary();
        sut["alpha"] = value;
        sut.Count.Should().Be(1);
        sut.IsSet("alpha").Should().BeTrue();
        sut["alpha"].Should().Be(value);
    }

    [Fact]
    public void ShouldBeCaseInsensitive()
    {
        var sut = new VariableDictionary();
        sut["alpha"] = "abc";
        sut["alpha"].Should().Be("abc");
        sut["Alpha"].Should().Be("abc");
        sut["ALPHA"].Should().Be("abc");
    }

    [Theory]
    [InlineData("abc", "def")]
    [InlineData("alpha", "alpha")]
    [InlineData(0, 0)]
    [InlineData(123, -456)]
    [InlineData(int.MinValue, int.MaxValue)]
    public void SetTwoValues(object value1, object value2)
    {
        var sut = new VariableDictionary();
        sut["alpha"] = value1;
        sut["beta"] = value2;
        sut.Count.Should().Be(2);
        sut.IsSet("alpha").Should().BeTrue();
        sut.IsSet("beta").Should().BeTrue();
        sut["alpha"].Should().Be(value1);
        sut["beta"].Should().Be(value2);
    }

    [Fact]
    public void NullShouldUnset()
    {
        var sut = new VariableDictionary();
        sut["alpha"] = 586;
        sut["alpha"] = null;
        sut.IsSet("alpha").Should().BeFalse();
        sut.Count.Should().Be(0);
    }
}
