namespace IS.Reading.Events;

public class TimedPauseEventTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(249)]
    [InlineData(4587)]
    [InlineData(5000)]
    public void Initialization(int intDuration)
    {
        var duration = TimeSpan.FromMilliseconds(intDuration);
        var sut = new TimedPauseEvent(duration);
        sut.Duration.Should().Be(duration);
        sut.ToString().Should().Be($"pause: {intDuration}");
    }
}
