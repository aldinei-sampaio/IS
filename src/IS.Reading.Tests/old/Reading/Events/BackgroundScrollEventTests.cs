using IS.Reading.State;

namespace IS.Reading.Events;

public class BackgroundScrollEventTests
{
    [Theory]
    [InlineData(BackgroundPosition.Left)]
    [InlineData(BackgroundPosition.Right)]
    public void Initialization(BackgroundPosition position)
    {
        var sut = new BackgroundScrollEvent(position);
        sut.Position.Should().Be(position);
        sut.ToString().Should().Be("bg scroll");
    }

    [Fact]
    public void PositionShouldNotBeUndefined()
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => new BackgroundScrollEvent(BackgroundPosition.Undefined)
        );
        ex.ParamName.Should().Be("position");
    }
}
