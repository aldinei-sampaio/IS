namespace IS.Reading.Parsing;

public class ParserDictionaryTests
{
    [Theory]
    [InlineData("alpha")]
    [InlineData("Alpha")]
    [InlineData("ALPHA")]
    public void DictionaryShouldBeCaseInsensitive(string lookFor)
    {
        var parser = Helper.FakeParser<INodeParser>("alpha");

        var sut = new ParserDictionary<INodeParser>();
        sut.Add(parser);

        sut[lookFor].Should().BeSameAs(parser);

        var result = sut.TryGet(lookFor, out var value);
        result.Should().BeTrue();
        value.Should().BeSameAs(parser);
    }
}
