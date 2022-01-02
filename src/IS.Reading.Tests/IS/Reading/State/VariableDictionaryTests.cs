using IS.Reading.Variables;

namespace IS.Reading.State;

public class VariableDictionaryTests
{
    [Fact]
    public void DictionaryShouldBeCaseInsensitive()
    {
        var sut = new VariableDictionary();
        sut["d1"] = 5;
        sut["d1"].Should().Be(5);
        sut["D1"].Should().Be(5);
    }

    [Fact]
    public void ZeroShouldNotBeStored()
    {
        var sut = new VariableDictionary();
        sut.Count.Should().Be(0);
        sut["batata"].Should().Be(0);
        sut["batata"] = 1;
        sut.Count.Should().Be(1);
        sut["banana"] = 1512;
        sut.Count.Should().Be(2);
        sut["batata"] = 0;
        sut.Count.Should().Be(1);
        sut["banana"] = 0;
        sut.Count.Should().Be(0);
    }
}
