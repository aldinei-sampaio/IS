namespace IS.Reading.Parsing.ConditionParsers.ConditionParserTests;

public class ParsingErrorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void EmptyText(string text)
    {
        var sut = new ConditionParser(A.Dummy<IWordReaderFactory>());
        var result = sut.Parse(text);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be("Era esperado um argumento com uma condição.");
    }

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
    [InlineData(WordType.Is, "is")]
    [InlineData(WordType.LowerThan, "<")]
    [InlineData(WordType.NotEqualsTo, "!=")]
    public void MisplacedKeyword(WordType wordType, string word)
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new() { wordType },
            Words = new() { word }
        };
        tester.TestError(string.Format(ConditionParser.MisplacedKeyword, word));
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
    [InlineData(WordType.Variable, WordType.EqualOrGreaterThan)]
    [InlineData(WordType.Variable, WordType.EqualOrLowerThan)]
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
        tester.TestError(ConditionParser.UnexpectedExpressionEnd);
    }

    [Theory]
    [InlineData(WordType.Variable)]
    [InlineData(WordType.Number)]
    [InlineData(WordType.String)]
    [InlineData(WordType.Null)]
    [InlineData(WordType.OpenParenthesys)]
    [InlineData(WordType.Not)]
    public void MissingLogicalOperator(WordType wordType)
    {
        var token = FakeToken(wordType);
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.Equals, WordType.Number, wordType},
            Words = new() { "a", "=", "1", token}
        };
        tester.TestError(string.Format(ConditionParser.MissingLogicalOperator, token));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("3000000000")]
    public void InvalidNumber(string value)
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Number },
            Words = new() { value }
        };
        tester.TestError(string.Format(ConditionParser.InvalidNumber, value));
    }

    [Theory]
    [InlineData(WordType.Variable)]
    [InlineData(WordType.String)]
    [InlineData(WordType.Number)]
    [InlineData(WordType.Null)]
    [InlineData(WordType.OpenParenthesys)]
    [InlineData(WordType.CloseParenthesys)]
    [InlineData(WordType.Equals)]
    [InlineData(WordType.NotEqualsTo)]
    [InlineData(WordType.EqualOrGreaterThan)]
    [InlineData(WordType.EqualOrLowerThan)]
    [InlineData(WordType.GreaterThan)]
    [InlineData(WordType.LowerThan)]
    [InlineData(WordType.And)]
    [InlineData(WordType.Or)]
    [InlineData(WordType.Not)]
    [InlineData(WordType.Comma)]
    [InlineData(WordType.Is)]
    public void InvalidTokenAfterNot(WordType wordType)
    {
        var token = FakeToken(wordType);
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.Not, wordType },
            Words = new() { "a", "", token }
        };
        tester.TestError(string.Format(ConditionParser.InvalidTokenAfterNot, token));
    }

    [Theory]
    [InlineData(WordType.Variable)]
    [InlineData(WordType.String)]
    [InlineData(WordType.Number)]
    [InlineData(WordType.OpenParenthesys)]
    [InlineData(WordType.CloseParenthesys)]
    [InlineData(WordType.Equals)]
    [InlineData(WordType.NotEqualsTo)]
    [InlineData(WordType.EqualOrGreaterThan)]
    [InlineData(WordType.EqualOrLowerThan)]
    [InlineData(WordType.GreaterThan)]
    [InlineData(WordType.LowerThan)]
    [InlineData(WordType.And)]
    [InlineData(WordType.Or)]
    [InlineData(WordType.Between)]
    [InlineData(WordType.Comma)]
    [InlineData(WordType.Is)]
    [InlineData(WordType.In)]
    public void InvalidTokenAfterIs(WordType wordType)
    {
        var token = FakeToken(wordType);
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.Is, wordType },
            Words = new() { "a", "", token }
        };
        tester.TestError(string.Format(ConditionParser.InvalidTokenAfterIs, token));
    }

    [Theory]
    [InlineData(WordType.Variable)]
    [InlineData(WordType.String)]
    [InlineData(WordType.Number)]
    [InlineData(WordType.OpenParenthesys)]
    [InlineData(WordType.CloseParenthesys)]
    [InlineData(WordType.Equals)]
    [InlineData(WordType.NotEqualsTo)]
    [InlineData(WordType.EqualOrGreaterThan)]
    [InlineData(WordType.EqualOrLowerThan)]
    [InlineData(WordType.GreaterThan)]
    [InlineData(WordType.LowerThan)]
    [InlineData(WordType.And)]
    [InlineData(WordType.Or)]
    [InlineData(WordType.Between)]
    [InlineData(WordType.Comma)]
    [InlineData(WordType.Is)]
    [InlineData(WordType.In)]
    [InlineData(WordType.Not)]
    public void InvalidTokenAfterIsNot(WordType wordType)
    {
        var token = FakeToken(wordType);
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.Is, WordType.Not, wordType },
            Words = new() { "a", "", "", token }
        };
        tester.TestError(string.Format(ConditionParser.InvalidTokenAfterIsNot, token));
    }

    [Theory]
    [InlineData(WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Invalid)]
    [InlineData(WordType.OpenParenthesys, WordType.Invalid)]
    [InlineData(WordType.Not, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Equals, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Equals, WordType.String, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.In, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.In, WordType.OpenParenthesys, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.In, WordType.OpenParenthesys, WordType.Number, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.In, WordType.OpenParenthesys, WordType.Number, WordType.Comma, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.In, WordType.OpenParenthesys, WordType.Number, WordType.CloseParenthesys, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Is, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Is, WordType.Null, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Is, WordType.Not, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Is, WordType.Not, WordType.Null, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Between, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Between, WordType.Number, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Between, WordType.Number, WordType.And, WordType.Invalid)]
    [InlineData(WordType.Variable, WordType.Between, WordType.Number, WordType.And, WordType.Number, WordType.Invalid)]
    public void InvalidToken(params WordType[] wordTypes)
    {
        var token = FakeToken(wordTypes[^1]);
        var tester = new ConditionParserTester
        {
            WordTypes = new(wordTypes),
            Words = new(wordTypes.Select(i => FakeToken(i)))
        };
        tester.TestError(string.Format(ConditionParser.InvalidToken, token));
    }

    [Theory]
    [InlineData(WordType.OpenParenthesys)]
    [InlineData(WordType.CloseParenthesys)]
    [InlineData(WordType.Equals)]
    [InlineData(WordType.NotEqualsTo)]
    [InlineData(WordType.EqualOrGreaterThan)]
    [InlineData(WordType.EqualOrLowerThan)]
    [InlineData(WordType.GreaterThan)]
    [InlineData(WordType.LowerThan)]
    [InlineData(WordType.And)]
    [InlineData(WordType.Or)]
    [InlineData(WordType.Not)]
    [InlineData(WordType.Between)]
    [InlineData(WordType.Comma)]
    [InlineData(WordType.Is)]
    [InlineData(WordType.In)]
    public void InvalidOperand(WordType wordType)
    {
        var token = FakeToken(wordType);
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.Equals, wordType },
            Words = new() { "a", "", token }
        };
        tester.TestError(string.Format(ConditionParser.InvalidOperand, token));
    }

    [Theory]
    [InlineData(WordType.Variable)]
    [InlineData(WordType.String)]
    [InlineData(WordType.Number)]
    [InlineData(WordType.Null)]
    [InlineData(WordType.OpenParenthesys)]
    [InlineData(WordType.CloseParenthesys)]
    [InlineData(WordType.And)]
    [InlineData(WordType.Or)]
    [InlineData(WordType.Comma)]
    public void InvalidOperator(WordType wordType)
    {
        var token = FakeToken(wordType);
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, wordType },
            Words = new() { "a", token }
        };
        tester.TestError(string.Format(ConditionParser.InvalidOperator, token));
    }

    [Theory]
    [InlineData(WordType.Variable)]
    [InlineData(WordType.String)]
    [InlineData(WordType.Number)]
    [InlineData(WordType.Null)]
    [InlineData(WordType.OpenParenthesys)]
    [InlineData(WordType.CloseParenthesys)]
    [InlineData(WordType.Equals)]
    [InlineData(WordType.NotEqualsTo)]
    [InlineData(WordType.EqualOrGreaterThan)]
    [InlineData(WordType.EqualOrLowerThan)]
    [InlineData(WordType.GreaterThan)]
    [InlineData(WordType.LowerThan)]
    [InlineData(WordType.Or)]
    [InlineData(WordType.Between)]
    [InlineData(WordType.Comma)]
    [InlineData(WordType.Is)]
    [InlineData(WordType.In)]
    [InlineData(WordType.Not)]
    public void BetweenWithoutAnd(WordType wordType)
    {
        var token = FakeToken(wordType);
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.Between, WordType.Number, wordType },
            Words = new() { "a", "", "1", token }
        };
        tester.TestError(string.Format(ConditionParser.BetweenWithoutAnd, token));
    }

    [Theory]
    [InlineData(WordType.Variable)]
    [InlineData(WordType.String)]
    [InlineData(WordType.Number)]
    [InlineData(WordType.Null)]
    [InlineData(WordType.CloseParenthesys)]
    [InlineData(WordType.Equals)]
    [InlineData(WordType.NotEqualsTo)]
    [InlineData(WordType.EqualOrGreaterThan)]
    [InlineData(WordType.EqualOrLowerThan)]
    [InlineData(WordType.GreaterThan)]
    [InlineData(WordType.LowerThan)]
    [InlineData(WordType.And)]
    [InlineData(WordType.Or)]
    [InlineData(WordType.Between)]
    [InlineData(WordType.Comma)]
    [InlineData(WordType.Is)]
    [InlineData(WordType.In)]
    [InlineData(WordType.Not)]
    public void InvalidTokenAfterIn(WordType wordType)
    {
        var token = FakeToken(wordType);
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.In, wordType },
            Words = new() { "a", "", token }
        };
        tester.TestError(string.Format(ConditionParser.InvalidTokenAfterIn, token));
    }

    [Theory]
    [InlineData(WordType.Variable)]
    [InlineData(WordType.String)]
    [InlineData(WordType.Number)]
    [InlineData(WordType.Null)]
    [InlineData(WordType.CloseParenthesys)]
    [InlineData(WordType.Equals)]
    [InlineData(WordType.NotEqualsTo)]
    [InlineData(WordType.EqualOrGreaterThan)]
    [InlineData(WordType.EqualOrLowerThan)]
    [InlineData(WordType.GreaterThan)]
    [InlineData(WordType.LowerThan)]
    [InlineData(WordType.And)]
    [InlineData(WordType.Or)]
    [InlineData(WordType.Between)]
    [InlineData(WordType.Comma)]
    [InlineData(WordType.Is)]
    [InlineData(WordType.In)]
    [InlineData(WordType.Not)]
    public void InvalidTokenAfterNotIn(WordType wordType)
    {
        var token = FakeToken(wordType);
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.Not, WordType.In, wordType },
            Words = new() { "a", "", "", token }
        };
        tester.TestError(string.Format(ConditionParser.InvalidTokenAfterIn, token));
    }

    [Theory]
    [InlineData(WordType.Variable)]
    [InlineData(WordType.String)]
    [InlineData(WordType.Number)]
    [InlineData(WordType.Null)]
    [InlineData(WordType.OpenParenthesys)]
    [InlineData(WordType.Equals)]
    [InlineData(WordType.NotEqualsTo)]
    [InlineData(WordType.EqualOrGreaterThan)]
    [InlineData(WordType.EqualOrLowerThan)]
    [InlineData(WordType.GreaterThan)]
    [InlineData(WordType.LowerThan)]
    [InlineData(WordType.And)]
    [InlineData(WordType.Or)]
    [InlineData(WordType.Between)]
    [InlineData(WordType.Is)]
    [InlineData(WordType.In)]
    [InlineData(WordType.Not)]
    public void InvalidTokenAfterInValue(WordType wordType)
    {
        var token = FakeToken(wordType);
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.In, WordType.OpenParenthesys, WordType.Number, wordType },
            Words = new() { "a", "", "", "1", token }
        };
        tester.TestError(string.Format(ConditionParser.InvalidTokenAfterInValue, token));
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
