using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing.Attributes;

public class WhenAttributeParserTests
{
    [Fact]
    public void ElementName()
    {
        var sut = new WhenAttributeParser(A.Dummy<IConditionParser>());
        sut.ElementName.Should().Be("when");
    }

    [Fact]
    public void ValidCondition()
    {
        var reader = CreateReader("<t when=\"abc\" />");
        var context = A.Fake<IParsingContext>(i => i.Strict());

        var condition = A.Dummy<ICondition>();

        var parser = A.Fake<IConditionParser>(i => i.Strict());
        A.CallTo(() => parser.Parse("abc")).Returns(condition);

        var sut = new WhenAttributeParser(parser);
        var result = sut.Parse(reader, context);
        result.Should().NotBeNull();
        result.Should().BeOfType<WhenAttribute>().Which.Condition.Should().BeSameAs(condition);
    }

    [Fact]
    public void InvalidCondition()
    {
        const string message = "Condição 'when' inválida.";

        var reader = CreateReader("<t when=\"abc\" />");
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var condition = A.Dummy<ICondition>();

        var parser = A.Fake<IConditionParser>(i => i.Strict());
        A.CallTo(() => parser.Parse("abc")).Returns(null);

        var sut = new WhenAttributeParser(parser);
        var result = sut.Parse(reader, context);
        result.Should().BeNull();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    private static XmlReader CreateReader(string xmlContents)
    {
        var textReader = new StringReader(xmlContents);
        var reader = XmlReader.Create(textReader);
        reader.MoveToContent();
        reader.MoveToFirstAttribute();
        return reader;
    }
}
