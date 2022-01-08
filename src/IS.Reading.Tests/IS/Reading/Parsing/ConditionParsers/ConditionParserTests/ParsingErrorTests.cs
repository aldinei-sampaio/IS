namespace IS.Reading.Parsing.ConditionParsers.ConditionParserTests;

public class ParsingErrorTests
{
    [Theory]
    [InlineData(WordType.Or, "or")]
    [InlineData(WordType.And, "and")]
    [InlineData(WordType.CloseParenthesys, ")")]
    [InlineData(WordType.Between, "between")]
    [InlineData(WordType.Comma, ",")]
    [InlineData(WordType.EqualOrGreaterThan, ">=")]
    [InlineData(WordType.EqualOrLowerThan, "<=")]
    [InlineData(WordType.Equals, "=")]
    [InlineData(WordType.GreaterThan, ">")]
    [InlineData(WordType.In, "in")]
    [InlineData(WordType.Invalid, "+")]
    [InlineData(WordType.Is, "is")]
    [InlineData(WordType.LowerThan, "<")]
    [InlineData(WordType.NotEqualsTo, "!=")]
    public void UnexpectedToken(WordType wordType, string word)
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new() { wordType },
            Words = new() { word }
        };
        tester.TestError($"'{word}' não é válido nesse ponto da expressão.");
    }

    [Theory]
    [InlineData(new WordType[] { })]
    [InlineData(WordType.Variable)]
    [InlineData(WordType.String)]
    [InlineData(WordType.Number)]
    [InlineData(WordType.Null)]
    [InlineData(WordType.Not)]
    [InlineData(WordType.Variable, WordType.Equals)]
    [InlineData(WordType.String, WordType.GreaterThan)]
    [InlineData(WordType.Number, WordType.LowerThan)]
    [InlineData(WordType.Null, WordType.NotEqualsTo)]
    [InlineData(WordType.OpenParenthesys, WordType.String)]
    [InlineData(WordType.Not, WordType.Variable)]
    [InlineData(WordType.String, WordType.In)]
    [InlineData(WordType.String, WordType.In, WordType.OpenParenthesys)]
    [InlineData(WordType.String, WordType.In, WordType.OpenParenthesys, WordType.Variable)]
    [InlineData(WordType.String, WordType.In, WordType.OpenParenthesys, WordType.Variable, WordType.Comma)]
    [InlineData(WordType.Number, WordType.Not)]
    [InlineData(WordType.Number, WordType.Not, WordType.In)]
    [InlineData(WordType.Number, WordType.Not, WordType.In, WordType.OpenParenthesys)]
    [InlineData(WordType.Number, WordType.Not, WordType.In, WordType.OpenParenthesys, WordType.Variable)]
    [InlineData(WordType.Number, WordType.Not, WordType.In, WordType.OpenParenthesys, WordType.Variable, WordType.Comma)]
    [InlineData(WordType.Variable, WordType.Is)]
    [InlineData(WordType.Variable, WordType.Is, WordType.Not)]
    [InlineData(WordType.Variable, WordType.Between)]
    [InlineData(WordType.Variable, WordType.Between, WordType.String)]
    [InlineData(WordType.Variable, WordType.Between, WordType.String, WordType.And)]
    [InlineData(WordType.Variable, WordType.Not, WordType.Between)]
    [InlineData(WordType.Variable, WordType.Not, WordType.Between, WordType.Number)]
    [InlineData(WordType.Variable, WordType.Not, WordType.Between, WordType.Number, WordType.And)]
    [InlineData(WordType.Variable, WordType.Equals, WordType.Number, WordType.And)]
    [InlineData(WordType.Variable, WordType.Equals, WordType.Number, WordType.Or)]
    [InlineData(WordType.Variable, WordType.Equals, WordType.Number, WordType.And, WordType.Variable)]
    [InlineData(WordType.Variable, WordType.Equals, WordType.Number, WordType.Or, WordType.Variable, WordType.NotEqualsTo)]
    [InlineData(WordType.OpenParenthesys)]
    [InlineData(WordType.OpenParenthesys, WordType.Variable)]
    [InlineData(WordType.OpenParenthesys, WordType.Variable, WordType.Equals)]
    [InlineData(WordType.OpenParenthesys, WordType.Variable, WordType.Equals, WordType.String)]
    [InlineData(WordType.OpenParenthesys, WordType.Variable, WordType.Equals, WordType.String, WordType.CloseParenthesys, WordType.And)]
    public void UnexpectedExpressionEnd(params WordType[] wordTypes)
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new(wordTypes),
            Words = new(wordTypes.Select(i => FakeToken(i)))
        };
        tester.TestError("Fim inesperado da expressão.");
    }

    private static string FakeToken(WordType wordType)
    {
        return wordType switch
        {
            WordType.Variable => "a",
            WordType.Number => "1",
            _ => ""
        };
    }
}
