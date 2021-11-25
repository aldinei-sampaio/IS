using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing;

public class StoryboardParserTests
{
    [Fact]
    public async Task SimpleParsing()
    {
        var reader = new StringReader("<storyboard />");
        var block = A.Dummy<IBlock>();

        var rootBlockParser = A.Fake<IRootBlockParser>();
        A.CallTo(() => rootBlockParser.ParseAsync(null, null)).WithAnyArguments().Returns(block);

        var sut = new StoryboardParser(rootBlockParser);
        var result = await sut.ParseAsync(reader);

        result.Should().NotBeNull();
        result.RootBlock.Should().BeSameAs(block);
        result.EnteredBlocks.Should().BeEmpty();
        result.CurrentBlock.Should().BeNull();
        result.CurrentNode.Should().BeNull();
    }

    [Fact]
    public async Task XmlReaderArgument()
    {
        var reader = new StringReader("<storyboard><abc /></storyboard>");
        var block = A.Dummy<IBlock>();

        var rootBlockParser = A.Fake<IRootBlockParser>();
        A.CallTo(() => rootBlockParser.ParseAsync(null, null)).WithAnyArguments()
            .ReturnsLazily(async i =>
            {
                var reader = i.GetArgument<XmlReader>(0);
                var context = i.GetArgument<IParsingContext>(1);

                (await reader.ReadAsync()).Should().BeTrue();
                reader.LocalName.Should().Be("abc");
                (await reader.ReadAsync()).Should().BeTrue();
                reader.NodeType.Should().Be(XmlNodeType.EndElement);
                (await reader.ReadAsync()).Should().BeFalse();

                return block;
            });

        var sut = new StoryboardParser(rootBlockParser);
        var result = await sut.ParseAsync(reader);

        result.Should().NotBeNull();
        result.RootBlock.Should().BeSameAs(block);
    }

    [Fact]
    public async Task ContextArgument()
    {
        var reader = new StringReader("<storyboard />");
        var rootBlockParser = A.Fake<IRootBlockParser>();
        A.CallTo(() => rootBlockParser.ParseAsync(null, null)).WithAnyArguments()
            .ReturnsLazily(i =>
            {
                var reader = i.GetArgument<XmlReader>(0);
                var context = i.GetArgument<IParsingContext>(1);

                context.LogError(reader, "Erro proposital");

                return Task.FromResult<IBlock>(null);
            });

        var sut = new StoryboardParser(rootBlockParser);

        var ex = await Assert.ThrowsAsync<ParsingException>(
            () => sut.ParseAsync(reader)
        );

        ex.Message.Should().Be("Linha 1: Erro proposital");
    }

    [Fact]
    public async Task LineNumberOnErrorMessages()
    {
        var reader = new StringReader("<storyboard>\r\n<a />\r\n<b />\r\n</storyboard>");
        var rootBlockParser = A.Fake<IRootBlockParser>();
        A.CallTo(() => rootBlockParser.ParseAsync(null, null)).WithAnyArguments()
            .ReturnsLazily(async i =>
            {
                var reader = i.GetArgument<XmlReader>(0);
                var context = i.GetArgument<IParsingContext>(1);

                while (await reader.ReadAsync())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                        context.LogError(reader, reader.LocalName);
                }

                return null;
            });

        var sut = new StoryboardParser(rootBlockParser);

        var ex = await Assert.ThrowsAsync<ParsingException>(
            () => sut.ParseAsync(reader)
        );

        ex.Message.Should().Be($"Linha 2: a{Environment.NewLine}Linha 3: b");
    }
}
