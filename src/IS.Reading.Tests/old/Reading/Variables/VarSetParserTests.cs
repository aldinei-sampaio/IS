namespace IS.Reading.Variables;

public class VarSetParserTests
{
    [Theory]
    [InlineData("alpha=0")]
    [InlineData("alpha=1")]
    [InlineData("alpha=-1")]
    [InlineData("alpha=''")]
    [InlineData("alpha='abc'")]
    [InlineData("Alpha = 0", "alpha=0")]
    [InlineData("ALPHA = 'abc'", "alpha='abc'")]
    [InlineData("beta_a=2147483647", "beta_a=2147483647")]
    [InlineData(" gamma  =  -2147483648", "gamma=-2147483648")]
    [InlineData("alpha=999999999")]
    [InlineData("alpha=-999999999")]
    [InlineData("alpha=1999999999")]
    [InlineData("alpha=-1999999999")]
    [InlineData("alpha=2099999999")]
    [InlineData("alpha=-2099999999")]
    [InlineData("alpha=2139999999")]
    [InlineData("alpha=-2139999999")]
    [InlineData("alpha=2146999999")]
    [InlineData("alpha=-2146999999")]
    [InlineData("alpha=2147399999")]
    [InlineData("alpha=-2147399999")]
    [InlineData("alpha=2147479999")]
    [InlineData("alpha=-2147479999")]
    [InlineData("alpha=2147482999")]
    [InlineData("alpha=-2147482999")]
    [InlineData("alpha=2147483599")]
    [InlineData("alpha=-2147483599")]
    [InlineData("alpha=2147483639")]
    [InlineData("alpha=-2147483639")]
    [InlineData("alpha=2147483647")]
    [InlineData("alpha=-2147483648")]
    public void ValidVarSet(params string[] args)
    {
        var expression = args[0];
        var expected = args.Length > 1 ? args[1] : expression;

        var sut = new VarSetParser();
        var result = sut.Parse(expression);
        result.Should().BeOfType<VarSet>()
            .Which.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("alpha+=1", "alpha++")]
    [InlineData("beta-=1", "beta--")]
    [InlineData("a++")]
    [InlineData("A++", "a++")]
    [InlineData("_++")]
    [InlineData("alpha++")]
    [InlineData("Alpha_Beta ++", "alpha_beta++")]
    [InlineData(" ALPHA  ++   ", "alpha++")]
    [InlineData("a--")]
    [InlineData("A--", "a--")]
    [InlineData("_--")]
    [InlineData("alpha--")]
    [InlineData("alpha --", "alpha--")]
    [InlineData("BETA+=1", "beta++")]
    [InlineData("b += 158", "b+=158")]
    [InlineData("b += -5", "b-=5")]
    [InlineData("Alpha_Beta -= 2", "alpha_beta-=2")]
    [InlineData("c -= -5", "c+=5")]
    public void ValidIntegerIncrement(params string[] args)
    {
        var expression = args[0];
        var expected = args.Length > 1 ? args[1] : expression;

        var sut = new VarSetParser();
        var result = sut.Parse(expression);
        result.Should().BeOfType<IntegerIncrement>()
            .Which.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("alpha")]
    [InlineData("alpha=")]
    [InlineData("alpha=2147483648")]
    [InlineData("alpha=-2147483649")]
    [InlineData("alpha=9999999999")]
    [InlineData("alpha=-9999999999")]
    [InlineData("alpha+=")]
    [InlineData("alpha-=")]
    [InlineData("alpha=beta")]
    [InlineData("+=1")]
    [InlineData("=1")]
    [InlineData("++")]
    [InlineData("'a'")]
    [InlineData("'abc' = 'abc'")]
    [InlineData("a=02147483647")]
    [InlineData("a=-02147483648")]
    [InlineData("a+=00000000000")]
    [InlineData("a-=00000000000")]
    public void InvalidPatterns(string expression)
    {
        var sut = new VarSetParser();
        var result = sut.Parse(expression);
        result.Should().BeNull();
    }
}
