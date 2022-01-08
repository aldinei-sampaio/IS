using IS.Reading.Conditions;

namespace IS.Reading.Parsing.ConditionParsers.ConditionParserTests;

public class HappyPathTests
{
    [Fact]
    public void VariableEqualsString()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.Equals, WordType.String },
            Words = new() { "campo", "", "valor" }
        };
        tester.Test<EqualsToCondition>("campo = 'valor'");
    }

    [Fact]
    public void VariableLowerThanNumber()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.LowerThan, WordType.Number },
            Words = new() { "var", "", "123" }
        };
        tester.Test<LowerThanCondition>("var < 123");
    }

    [Fact]
    public void StringGreaterThanString()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.String, WordType.GreaterThan, WordType.String },
            Words = new() { "abc", "", "def" }
        };
        tester.Test<GreaterThanCondition>("'abc' > 'def'");
    }

    [Fact]
    public void NumberDifferentFromNull()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Number, WordType.NotEqualsTo, WordType.Null },
            Words = new() { "123", "", "" }
        };
        tester.Test<NotEqualsToCondition>("123 != Null");
    }

    [Fact]
    public void StringEqualsOrLowerThanVariable()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.String, WordType.EqualOrLowerThan, WordType.Variable },
            Words = new() { "abc", "", "campo" }
        };
        tester.Test<EqualOrLowerThanCondition>("'abc' <= campo");
    }

    [Fact]
    public void VariableEqualsOrGreaterThanVariable()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new() { WordType.Variable, WordType.EqualOrGreaterThan, WordType.Variable },
            Words = new() { "campo1", "", "campo2" }
        };
        tester.Test<EqualOrGreaterThanCondition>("campo1 >= campo2");
    }

    [Fact]
    public void VariableIn()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Variable,
                WordType.In,
                WordType.OpenParenthesys,
                WordType.Variable,
                WordType.Comma,
                WordType.Number,
                WordType.Comma,
                WordType.String,
                WordType.Comma,
                WordType.Null,
                WordType.CloseParenthesys
            },
            Words = new() { "campoa", "", "", "campob", "", "123", "", "abc", "", "", "" }
        };
        tester.Test<InCondition>("campoa In (campob, 123, 'abc', Null)");
    }

    [Fact]
    public void NumberNotIn()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Number,
                WordType.Not,
                WordType.In,
                WordType.OpenParenthesys,
                WordType.Variable,
                WordType.Comma,
                WordType.Variable,
                WordType.CloseParenthesys
            },
            Words = new() { "-12", "", "", "", "alpha", "", "beta", "" }
        };
        tester.Test<NotInCondition>("-12 Not In (alpha, beta)");
    }

    [Fact]
    public void VariableIsNull()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Variable,
                WordType.Is,
                WordType.Null
            },
            Words = new() { "omega", "", "" }
        };
        tester.Test<IsNullCondition>("omega Is Null");
    }

    [Fact]
    public void VariableIsNotNull()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Variable,
                WordType.Is,
                WordType.Not,
                WordType.Null
            },
            Words = new() { "rho", "", "", "" }
        };
        tester.Test<IsNotNullCondition>("rho Is Not Null");
    }

    [Fact]
    public void VariableBetweenNumbers()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Variable,
                WordType.Between,
                WordType.Number,
                WordType.And,
                WordType.Number
            },
            Words = new() { "delta", "", "0", "", "10" }
        };
        tester.Test<BetweenCondition>("delta Between 0 And 10");
    }

    [Fact]
    public void StringNotBetweenStringAndVariable()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.String,
                WordType.Not,
                WordType.Between,
                WordType.String,
                WordType.And,
                WordType.Variable
            },
            Words = new() { "abc", "", "", "def", "", "gamma" }
        };
        tester.Test<NotBetweenCondition>("'abc' Not Between 'def' And gamma");
    }

    [Fact]
    public void AndCondition()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Variable,
                WordType.Equals,
                WordType.Number,
                WordType.And,
                WordType.Variable,
                WordType.NotEqualsTo,
                WordType.String
            },
            Words = new() { "epsilon", "", "0", "", "teta", "", "def" }
        };
        tester.Test<AndCondition>("(epsilon = 0) And (teta != 'def')");
    }

    [Fact]
    public void OrCondition()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.String,
                WordType.GreaterThan,
                WordType.Variable,
                WordType.Or,
                WordType.Variable,
                WordType.In,
                WordType.OpenParenthesys,
                WordType.String,
                WordType.Comma,
                WordType.String,
                WordType.CloseParenthesys
            },
            Words = new() { "abc", "", "alpha", "", "beta", "", "", "def", "", "ghi", "" }
        };
        tester.Test<OrCondition>("('abc' > alpha) Or (beta In ('def', 'ghi'))");
    }

    [Fact]
    public void NotCondition()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Not,
                WordType.Variable,
                WordType.Is,
                WordType.Null
            },
            Words = new() { "", "a", "", "" }
        };
        tester.Test<NotCondition>("Not (a Is Null)");
    }

    [Fact]
    public void MultipleLogicalConditions()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Variable,
                WordType.Equals,
                WordType.String,
                WordType.And,
                WordType.Variable,
                WordType.Is,
                WordType.Not,
                WordType.Null,
                WordType.Or,
                WordType.Not,
                WordType.Variable,
                WordType.Between,
                WordType.Number,
                WordType.And,
                WordType.Number
            },
            Words = new() { "a", "", "x", "", "b", "", "", "", "", "", "c", "", "1", "", "5" }
        };
        tester.Test<OrCondition>("((a = 'x') And (b Is Not Null)) Or (Not (c Between 1 And 5))");
    }

    [Fact]
    public void Parenthesys1()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.OpenParenthesys,
                WordType.Variable,
                WordType.Is,
                WordType.Null,
                WordType.CloseParenthesys
            },
            Words = new() { "", "a", "", "", "" }
        };
        tester.Test<IsNullCondition>("a Is Null");
    }

    [Fact]
    public void Parenthesys2()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Variable,
                WordType.Equals,
                WordType.String,
                WordType.Or,
                WordType.OpenParenthesys,
                WordType.Variable,
                WordType.Is,
                WordType.Null,
                WordType.CloseParenthesys
            },
            Words = new() { "a", "", "x", "", "", "a", "", "", "" }
        };
        tester.Test<OrCondition>("(a = 'x') Or (a Is Null)");
    }

    [Fact]
    public void Parenthesys3()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Variable,
                WordType.Equals,
                WordType.String,
                WordType.Or,
                WordType.OpenParenthesys,
                WordType.Variable,
                WordType.Is,
                WordType.Null,
                WordType.And,
                WordType.Variable,
                WordType.Equals,
                WordType.Number,
                WordType.CloseParenthesys
            },
            Words = new() { "a", "", "x", "", "", "a", "", "", "", "b", "", "0", "" }
        };
        tester.Test<OrCondition>("(a = 'x') Or ((a Is Null) And (b = 0))");
    }

    [Fact]
    public void Parenthesys4()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Variable,
                WordType.Equals,
                WordType.String,
                WordType.And,
                WordType.OpenParenthesys,
                WordType.Variable,
                WordType.Is,
                WordType.Not,
                WordType.Null,
                WordType.Or,
                WordType.Not,
                WordType.Variable,
                WordType.Between,
                WordType.Number,
                WordType.And,
                WordType.Number,
                WordType.CloseParenthesys
            },
            Words = new() { "a", "", "x", "", "", "b", "", "", "", "", "", "c", "", "1", "", "5", "" }
        };
        tester.Test<AndCondition>("(a = 'x') And ((b Is Not Null) Or (Not (c Between 1 And 5)))");
    }

    [Fact]
    public void Parenthesys5()
    {
        var tester = new ConditionParserTester
        {
            WordTypes = new()
            {
                WordType.Not,
                WordType.OpenParenthesys,
                WordType.Variable,
                WordType.Equals,
                WordType.Number,
                WordType.Or,
                WordType.Variable,
                WordType.Equals,
                WordType.Number,
                WordType.CloseParenthesys
            },
            Words = new() { "", "", "a", "", "0", "", "a", "", "1", "" }
        };
        tester.Test<NotCondition>("Not ((a = 0) Or (a = 1))");
    }
}
