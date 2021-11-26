using IS.Reading.Events;
using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing;

public class StoryboardParserTests
{
    private readonly IParsingContext parsingContext;
    private readonly IRootBlockParser rootBlockParser;
    private readonly ISceneNavigator sceneNavigator;
    private readonly IEventManager eventManager;
    private readonly StoryboardParser sut;

    public StoryboardParserTests()
    {
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        rootBlockParser = A.Fake<IRootBlockParser>(i => i.Strict());
        sceneNavigator = A.Fake<ISceneNavigator>(i => i.Strict());
        eventManager = A.Fake<IEventManager>(i => i.Strict());

        var serviceProvider = A.Fake<IServiceProvider>(i => i.Strict());
        A.CallTo(() => serviceProvider.GetService(typeof(IParsingContext))).Returns(parsingContext);
        A.CallTo(() => serviceProvider.GetService(typeof(IRootBlockParser))).Returns(rootBlockParser);
        A.CallTo(() => serviceProvider.GetService(typeof(ISceneNavigator))).Returns(sceneNavigator);
        A.CallTo(() => serviceProvider.GetService(typeof(IEventManager))).Returns(eventManager);
        sut = new StoryboardParser(serviceProvider);
    }

    [Fact]
    public async Task SimpleParsing()
    {
        var reader = new StringReader("<storyboard />");
        var block = A.Dummy<IBlock>();
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => rootBlockParser.ParseAsync(A<XmlReader>.Ignored, parsingContext)).Returns(block);

        var result = (Storyboard)await sut.ParseAsync(reader);

        result.Should().NotBeNull();
        result.Events.Should().BeSameAs(eventManager);
        result.NavigationContext.RootBlock.Should().BeSameAs(block);
        result.NavigationContext.EnteredBlocks.Should().BeEmpty();
        result.NavigationContext.CurrentBlock.Should().BeNull();
        result.NavigationContext.CurrentNode.Should().BeNull();
    }

    [Fact]
    public async Task XmlReaderArgument()
    {
        var reader = new StringReader("<storyboard><abc /></storyboard>");
        var block = A.Dummy<IBlock>();

        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => rootBlockParser.ParseAsync(A<XmlReader>.Ignored, parsingContext))
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

        var result = (Storyboard)await sut.ParseAsync(reader);

        result.Should().NotBeNull();
        result.NavigationContext.RootBlock.Should().BeSameAs(block);
    }

    [Fact]
    public async Task ContextArgument()
    {
        var reader = new StringReader("<storyboard />");

        A.CallTo(() => parsingContext.IsSuccess).Returns(false);
        A.CallTo(() => parsingContext.LogError(A<XmlReader>.Ignored, "Erro proposital")).DoesNothing();
        A.CallTo(() => parsingContext.ToString()).Returns("Erro proposital");

        A.CallTo(() => rootBlockParser.ParseAsync(A<XmlReader>.Ignored, parsingContext))
            .ReturnsLazily(i =>
            {
                var reader = i.GetArgument<XmlReader>(0);
                var context = i.GetArgument<IParsingContext>(1);

                context.LogError(reader, "Erro proposital");

                return Task.FromResult<IBlock>(null);
            });

        var ex = await Assert.ThrowsAsync<ParsingException>(
            () => sut.ParseAsync(reader)
        );

        ex.Message.Should().Be("Erro proposital");
    }

    [Fact]
    public async Task WhenIsSuccessIsTrueBlockShouldNotBeNull()
    {
        var reader = new StringReader("<storyboard />");

        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        A.CallTo(() => rootBlockParser.ParseAsync(A<XmlReader>.Ignored, parsingContext))
            .Returns(Task.FromResult<IBlock>(null));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => sut.ParseAsync(reader)
        );
    }
}
