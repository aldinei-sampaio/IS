namespace IS.Reading.Parsing.ConditionParsers;

public class WordReaderTests
{
    [Theory]
    [InlineData("a=b", "a", "b")]
    [InlineData("alpha =beta", "alpha", "beta")]
    [InlineData("OMEGA = PI", "omega", "pi")]
    [InlineData("Nome1= Nome2", "nome1", "nome2")]
    [InlineData("  Hoje   =    Agora     ", "hoje", "agora")]
    public void IdentifierEqualsIdentifier(string text, string identifier1, string identifier2)
    {
        var sut = new WordReader(text);
        NextWord(sut, WordType.Identifier, identifier1);
        NextWord(sut, WordType.Equals);
        NextWord(sut, WordType.Identifier, identifier2);
        sut.Read().Should().BeFalse();
    }

    [Fact]
    public void Numbers()
    {
        var sut = new WordReader("1 57 609 -2 0");
        NextWord(sut, WordType.Number, "1");
        NextWord(sut, WordType.Number, "57");
        NextWord(sut, WordType.Number, "609");
        NextWord(sut, WordType.Number, "-2");
        NextWord(sut, WordType.Number, "0");
        sut.Read().Should().BeFalse();
    }

    [Fact]
    public void Strings()
    {
        var sut = new WordReader("'a' 'Alpha' ''");
        NextWord(sut, WordType.String, "a");
        NextWord(sut, WordType.String, "Alpha");
        NextWord(sut, WordType.String, "");
        sut.Read().Should().BeFalse();
    }

    [Fact]
    public void Operators()
    {
        var sut = new WordReader("= != < > <= >= <> ( )");
        NextWord(sut, WordType.Equals);
        NextWord(sut, WordType.Different);
        NextWord(sut, WordType.LowerThan);
        NextWord(sut, WordType.GreaterThan);
        NextWord(sut, WordType.EqualOrLowerThan);
        NextWord(sut, WordType.EqualOrGreaterThan);
        NextWord(sut, WordType.Different);
        NextWord(sut, WordType.OpenParenthesys);
        NextWord(sut, WordType.CloseParenthesys);
        sut.Read().Should().BeFalse();
    }

    [Fact]
    public void KeyWords()
    {
        var sut = new WordReader("and,OR Not in Between IS Null");
        NextWord(sut, WordType.And);
        NextWord(sut, WordType.Comma);
        NextWord(sut, WordType.Or);
        NextWord(sut, WordType.Not);
        NextWord(sut, WordType.In);
        NextWord(sut, WordType.Between);
        NextWord(sut, WordType.Is);
        NextWord(sut, WordType.Null);
        sut.Read().Should().BeFalse();
    }

    private static void NextWord(WordReader sut, WordType wordType, string word = null)
    {
        sut.Read().Should().BeTrue();
        sut.WordType.Should().Be(wordType);
        if (word is not null)
            sut.Word.Should().Be(word);
    }
}
