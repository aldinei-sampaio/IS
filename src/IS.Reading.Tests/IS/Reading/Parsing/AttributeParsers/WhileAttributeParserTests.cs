using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing.AttributeParsers;

public class WhileAttributeParserTests
{
    [Fact]
    public void AttributetName()
    {
        var sut = new WhileAttributeParser(A.Dummy<IConditionParser>());
        sut.Name.Should().Be("while");
    }

    [Fact]
    public void ValidCondition()
    {
        var reader = CreateReader("<t while=\"abc\" />");
        var context = A.Fake<IParsingContext>(i => i.Strict());

        var condition = A.Dummy<ICondition>();

        var parser = A.Fake<IConditionParser>(i => i.Strict());
        A.CallTo(() => parser.Parse("abc")).Returns(condition);

        var sut = new WhileAttributeParser(parser);
        var result = sut.Parse(reader, context);
        result.Should().NotBeNull();
        result.Should().BeOfType<WhileAttribute>().Which.Condition.Should().BeSameAs(condition);
    }

    [Fact]
    public void InvalidCondition()
    {
        const string message = "Condição 'while' inválida.";

        var reader = CreateReader("<t while=\"abc\" />");
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var condition = A.Dummy<ICondition>();

        var parser = A.Fake<IConditionParser>(i => i.Strict());
        A.CallTo(() => parser.Parse("abc")).Returns(null);

        var sut = new WhileAttributeParser(parser);
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
