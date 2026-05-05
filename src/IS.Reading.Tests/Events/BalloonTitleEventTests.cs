namespace IS.Reading.Events;

public class BalloonTitleEventTests
{
    [Theory]
    [InlineData("", "title unset")]
    [InlineData("???", "title: ???")]
    [InlineData("Adalberto", "title: Adalberto")]
    public void Initialization(string text, string toString)
    {
        var sut = new BalloonTitleEvent(text);
        sut.Text.Should().Be(text);
        sut.ToString().Should().Be(toString);
    }
}