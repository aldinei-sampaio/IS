using IS.Reading.Conditions;
using IS.Reading.Parsing.ConditionParsers;
using System.Xml;

namespace IS.Reading.Parsing.AttributeParsers;

public class WhenAttributeParserTests
{
    [Fact]
    public void AttributeName()
    {
        var sut = new WhenAttributeParser(A.Dummy<IConditionParser>());
        sut.Name.Should().Be("when");
    }

    [Fact]
    public void ValidCondition()
    {
        var reader = CreateReader("<t when=\"abc\" />");
        var context = A.Fake<IParsingContext>(i => i.Strict());

        var parsedCondition = A.Dummy<IParsedCondition>();

        var parser = A.Fake<IConditionParser>(i => i.Strict());
        A.CallTo(() => parser.Parse("abc")).Returns(parsedCondition);

        var sut = new WhenAttributeParser(parser);
        var result = sut.Parse(reader, context);
        result.Should().BeOfType<WhenAttribute>()
            .Which.Condition.Should().BeSameAs(parsedCondition.Condition);
    }

    [Fact]
    public void InvalidCondition()
    {
        const string message = "Condição 'when' inválida. Gibberish";

        var reader = CreateReader("<t when=\"abc\" />");
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var parsedCondition = A.Dummy<IParsedCondition>();
        A.CallTo(() => parsedCondition.Condition).Returns(null);
        A.CallTo(() => parsedCondition.Message).Returns("Gibberish");

        var parser = A.Fake<IConditionParser>(i => i.Strict());
        A.CallTo(() => parser.Parse("abc")).Returns(parsedCondition);

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
