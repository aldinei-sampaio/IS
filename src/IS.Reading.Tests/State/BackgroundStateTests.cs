namespace IS.Reading.State;

public class BackgroundStateTests
{
    [Theory]
    [InlineData("", BackgroundType.Undefined, BackgroundPosition.Undefined, "bg empty")]
    [InlineData("", BackgroundType.Color, BackgroundPosition.Undefined, "bg color: ")]
    [InlineData("abc", BackgroundType.Color, BackgroundPosition.Undefined, "bg color: abc")]
    [InlineData("", BackgroundType.Image, BackgroundPosition.Undefined, "bg: ")]
    [InlineData("def", BackgroundType.Image, BackgroundPosition.Undefined, "bg: def")]
    [InlineData("", BackgroundType.Image, BackgroundPosition.Left, "bg left: ")]
    [InlineData("ghi", BackgroundType.Image, BackgroundPosition.Left, "bg left: ghi")]
    [InlineData("", BackgroundType.Image, BackgroundPosition.Right, "bg right: ")]
    [InlineData("jkl", BackgroundType.Image, BackgroundPosition.Right, "bg right: jkl")]
    public void ToStringTest(string name, BackgroundType type, BackgroundPosition position, string expected)
    {
        var sut = new BackgroundState(name, type, position);
        sut.ToString().Should().Be(expected);
    }
}
