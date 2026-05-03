namespace IS.Reading.Navigation;

public class RandomizerTests
{
    [Fact]
    public void Shuffle()
    {
        var list = new List<int> { 1, 2, 3, 4, 5 };

        Randomizer.Seed(0);
        var sut = new Randomizer();

        var result = sut.Shuffle(list);
        result.Should().ContainInOrder(5, 4, 1, 3, 2);

        result = sut.Shuffle(list);
        result.Should().ContainInOrder(5, 3, 1, 2, 4);

        result = sut.Shuffle(list);
        result.Should().ContainInOrder(1, 2, 4, 3, 5);

        result = sut.Shuffle(list);
        result.Should().ContainInOrder(1, 5, 4, 2, 3);

        result = sut.Shuffle(list);
        result.Should().ContainInOrder(4, 5, 1, 2, 3);

        result = sut.Shuffle(list);
        result.Should().ContainInOrder(5, 1, 4, 3, 2);

        result = sut.Shuffle(list);
        result.Should().ContainInOrder(1, 3, 2, 5, 4);

        result = sut.Shuffle(list);
        result.Should().ContainInOrder(2, 4, 3, 1, 5);

        result = sut.Shuffle(list);
        result.Should().ContainInOrder(3, 4, 1, 2, 5);

    }
}
