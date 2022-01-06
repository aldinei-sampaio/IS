using IS.Reading.Conditions;

namespace IS.Reading.Parsing.ConditionParsers;

internal class ConditionParserTester : IWordReader
{
    public List<WordType> WordTypes { get; set; }
    public List<string> Words { get; set;  } 

    private int current = -1;

    public bool AtEnd => current < 0 || current >= WordTypes.Count;

    public string Word => Words[current];

    public WordType WordType => WordTypes[current];

    public bool Read()
    {
        if (current >= WordTypes.Count)
            return false;
        current++;
        return true;
    }

    public void Test<T>(string expected) where T : ICondition
    {
        var wordReaderFactory = A.Fake<IWordReaderFactory>(i => i.Strict());
        A.CallTo(() => wordReaderFactory.Create("Gibberish")).Returns(this);
        var sut = new ConditionParser(wordReaderFactory);
        var result = sut.Parse("Gibberish");
        result.Condition.Should().BeOfType<T>()
            .Which.ToString().Should().Be(expected);
    }
}

public class ConditionParserTests
{
    [Fact]
    public void IdentifierEqualsString()
    {
        var wordReader = new ConditionParserTester
        {
            WordTypes = new() { WordType.Identifier, WordType.Equals, WordType.String },
            Words = new() { "campo", "", "valor" }
        };
        wordReader.Test<EqualsToCondition>("campo = 'valor'");
    }

    [Fact]
    public void IdentifierLowerThanNumber()
    {
        var wordReader = new ConditionParserTester
        {
            WordTypes = new() { WordType.Identifier, WordType.LowerThan, WordType.Number },
            Words = new() { "var", "", "123" }
        };
        wordReader.Test<LowerThanCondition>("var < 123");
    }

    [Fact]
    public void StringGreaterThanString()
    {
        var wordReader = new ConditionParserTester
        {
            WordTypes = new() { WordType.String, WordType.GreaterThan, WordType.String },
            Words = new() { "abc", "", "def" }
        };
        wordReader.Test<GreaterThanCondition>("'abc' > 'def'");
    }
}
