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
}
