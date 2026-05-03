using System.Collections;

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

        var sut = new ParserDictionary<INodeParser> { parser };

        sut.Count.Should().Be(1);
        sut[lookFor].Should().BeSameAs(parser);

        var result = sut[lookFor];
        result.Should().BeSameAs(parser);
    }

    [Fact]
    public void DefaultMethodShouldReturnNullWhenKeyNotExists()
    {
        var sut = new ParserDictionary<INodeParser>();
        sut.Should().BeEmpty();
        sut["chaveinexistente"].Should().BeNull();
    }

    [Fact]
    public void NameRegex()
    {
        var parser1 = A.Dummy<IParser>();
        A.CallTo(() => parser1.Name).Returns(string.Empty);
        A.CallTo(() => parser1.NameRegex).Returns(@"^[a-z]$");

        var parser2 = Helper.FakeParser<IParser>("abc");

        var sut = new ParserDictionary<IParser> { parser1, parser2 };

        sut.Count.Should().Be(2);
        sut["a"].Should().Be(parser1);
        sut["b"].Should().Be(parser1);
        sut["c"].Should().Be(parser1);
        sut["z"].Should().Be(parser1);
        sut["abc"].Should().Be(parser2);
    }

    [Fact]
    public void GenericEnumeration()
    {
        var parser1 = Helper.FakeParser<INodeParser>("alpha");
        var parser2 = Helper.FakeParser<INodeParser>("", "(a|b|c)");
        var parser3 = Helper.FakeParser<INodeParser>("beta");
        var parser4 = Helper.FakeParser<INodeParser>("", "(if|while)");

        var sut = new ParserDictionary<INodeParser> { parser1, parser2, parser3, parser4 };
        sut.Count.Should().Be(4);

        using var e = sut.GetEnumerator();
        e.MoveNext().Should().BeTrue();
        e.Current.Should().BeSameAs(parser1);
        e.MoveNext().Should().BeTrue();
        e.Current.Should().BeSameAs(parser2);
        e.MoveNext().Should().BeTrue();
        e.Current.Should().BeSameAs(parser3);
        e.MoveNext().Should().BeTrue();
        e.Current.Should().BeSameAs(parser4);
        e.MoveNext().Should().BeFalse();
    }

    [Fact]
    public void NonGenericEnumeration()
    {
        var parser1 = Helper.FakeParser<INodeParser>("alpha");
        var parser2 = Helper.FakeParser<INodeParser>("", "(a|b|c)");
        var parser3 = Helper.FakeParser<INodeParser>("beta");
        var parser4 = Helper.FakeParser<INodeParser>("", "(if|while)");

        var sut = new ParserDictionary<INodeParser> { parser1, parser2, parser3, parser4 };
        sut.Count.Should().Be(4);

        var e = ((IEnumerable)sut).GetEnumerator();
        e.MoveNext().Should().BeTrue();
        e.Current.Should().BeSameAs(parser1);
        e.MoveNext().Should().BeTrue();
        e.Current.Should().BeSameAs(parser2);
        e.MoveNext().Should().BeTrue();
        e.Current.Should().BeSameAs(parser3);
        e.MoveNext().Should().BeTrue();
        e.Current.Should().BeSameAs(parser4);
        e.MoveNext().Should().BeFalse();
    }
}
