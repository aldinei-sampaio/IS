using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class VarSetTextParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;

    public VarSetTextParserTests()
    {
        reader = A.Fake<XmlReader>(i => i.Strict());
        context = A.Fake<IParsingContext>(i => i.Strict());
    }

    private void MustBeValid(string expression)
    {
        var sut = new VarSetTextParser();
        var result = sut.Parse(reader, context, expression);
        result.Should().Be(expression);
    }

    [Theory]
    [InlineData("alpha=1")]
    [InlineData(" alpha  =   1    ")]
    [InlineData("BETA+=1")]
    [InlineData("b=-5")]
    [InlineData("b += -5")]
    [InlineData("Alpha_Beta -= 2")]
    [InlineData("alpha=0")]
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
    public void ValidIntegerSetPatterns(string expression) 
        => MustBeValid(expression);

    [Theory]
    [InlineData("a++")]
    [InlineData("A++")]
    [InlineData("_++")]
    [InlineData("alpha++")]
    [InlineData("Alpha_Beta ++")]
    [InlineData(" ALPHA  ++   ")]
    [InlineData("a--")]
    [InlineData("A--")]
    [InlineData("_--")]
    [InlineData("alpha--")]
    [InlineData("alpha --")]
    public void ValidIntegerIncrementPatterns(string expression) 
        => MustBeValid(expression);

    [Theory]
    [InlineData("alpha='a'")]
    [InlineData("Alpha = 'abc'")]
    [InlineData(" ALPHA  =   'ABC'    ")]
    [InlineData("alpha_beta='Este texto pode conter qualquer coisa, exceto apóstrofo. 1234567890 =><*&¨%#¨@!)(+-_*#&@[]'")]
    public void ValidStringSetPatterns(string expression)
        => MustBeValid(expression);

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
    public void InvalidPatterns(string expression)
    {
        const string message = "Expressão 'Set' inválida.";

        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var sut = new VarSetTextParser();
        sut.Parse(reader, context, expression);

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }
}
