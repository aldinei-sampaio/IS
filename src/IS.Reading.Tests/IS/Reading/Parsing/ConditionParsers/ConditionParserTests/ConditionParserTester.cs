using IS.Reading.Conditions;

namespace IS.Reading.Parsing.ConditionParsers.ConditionParserTests;

internal class ConditionParserTester : IWordReader
{
    public List<WordType> WordTypes { get; set; }
    public List<string> Words { get; set; }

    private int current = -1;

    public bool AtEnd => current < 0 || current >= WordTypes.Count;

    public string Word => Words[current];

    public WordType WordType => WordTypes[current];

    public bool Read()
    {
        if (current >= WordTypes.Count)
            return false;
        current++;
        return current < WordTypes.Count;
    }

    public void Test<T>(string expected) where T : ICondition
    {
        var result = ExecuteParsing();
        result.IsOk.Should().BeTrue();
        result.Value.Should().BeOfType<T>()
            .Which.ToString().Should().Be(expected);
    }

    private Result<ICondition> ExecuteParsing()
    {
        var wordReaderFactory = A.Fake<IWordReaderFactory>(i => i.Strict());
        A.CallTo(() => wordReaderFactory.Create("Gibberish")).Returns(this);
        var sut = new ConditionParser(wordReaderFactory);
        return sut.Parse("Gibberish");
    }

    public void TestError(string errorMessage)
    {
        var result = ExecuteParsing();
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }
}
