using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;
using IS.Reading.Variables;
using System.Xml;

namespace IS.Reading.Parsing;

public class StoryboardParserTests
{
    private readonly List<INode> dismissNodes;
    private readonly IParsingContext parsingContext;
    private readonly IRootBlockParser rootBlockParser;
    private readonly ISceneNavigator sceneNavigator;
    private readonly IEventManager eventManager;
    private readonly IRandomizer randomizer;
    private readonly INavigationState navigationState;
    private readonly IVariableDictionary variableDictionary;
    private readonly StoryboardParser sut;

    public StoryboardParserTests()
    {
        dismissNodes = new();
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => parsingContext.RegisterDismissNode(A<INode>.Ignored))
            .Invokes(i => dismissNodes.Add(i.Arguments.Get<INode>(0)));

        rootBlockParser = A.Fake<IRootBlockParser>(i => i.Strict());
        sceneNavigator = A.Fake<ISceneNavigator>(i => i.Strict());
        eventManager = A.Fake<IEventManager>(i => i.Strict());
        randomizer = A.Fake<IRandomizer>(i => i.Strict());
        navigationState = A.Fake<INavigationState>(i => i.Strict());
        variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());

        var serviceProvider = A.Fake<IServiceProvider>(i => i.Strict());
        A.CallTo(() => serviceProvider.GetService(typeof(IParsingContext))).Returns(parsingContext);
        A.CallTo(() => serviceProvider.GetService(typeof(IRootBlockParser))).Returns(rootBlockParser);
        A.CallTo(() => serviceProvider.GetService(typeof(ISceneNavigator))).Returns(sceneNavigator);
        A.CallTo(() => serviceProvider.GetService(typeof(IEventManager))).Returns(eventManager);
        A.CallTo(() => serviceProvider.GetService(typeof(IRandomizer))).Returns(randomizer);
        A.CallTo(() => serviceProvider.GetService(typeof(INavigationState))).Returns(navigationState);
        A.CallTo(() => serviceProvider.GetService(typeof(IVariableDictionary))).Returns(variableDictionary);
        sut = new StoryboardParser(serviceProvider);
    }

    [Fact]
    public async Task SimpleParsing()
    {
        var reader = new StringReader("<storyboard />");
        var block = A.Dummy<IBlock>();
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => parsingContext.DismissNodes).Returns(Enumerable.Empty<INode>());
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
        A.CallTo(() => parsingContext.DismissNodes).Returns(Enumerable.Empty<INode>());
        A.CallTo(() => rootBlockParser.ParseAsync(A<XmlReader>.Ignored, parsingContext))
            .ReturnsLazily(async i =>
            {
                var reader = i.GetArgument<XmlReader>(0);
                var context = i.GetArgument<IParsingContext>(1);

                reader.ReadState.Should().Be(ReadState.Initial);
                await reader.MoveToContentAsync();
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
        A.CallTo(() => parsingContext.ToString()).Returns("Erro proposital");
        A.CallTo(() => parsingContext.LogError(A<XmlReader>.Ignored, "Erro proposital")).DoesNothing();

        A.CallTo(() => rootBlockParser.ParseAsync(A<XmlReader>.Ignored, parsingContext))
            .ReturnsLazily(i =>
            {
                var reader = i.GetArgument<XmlReader>(0);
                var context = i.GetArgument<IParsingContext>(1);

                context.LogError(reader, "Erro proposital");

                return Task.FromResult(A.Dummy<IBlock>());
            });

        var ex = await Assert.ThrowsAsync<ParsingException>(
            () => sut.ParseAsync(reader)
        );

        ex.Message.Should().Be("Erro proposital");
    }

    [Fact]
    public async Task DismissNodesShouldBeAppendedToEndOfTheStoryboard()
    {
        var reader = new StringReader("<storyboard><abc /></storyboard>");

        var normalNode = A.Dummy<INode>();
        var dismissNode = A.Dummy<INode>();
        
        var block = A.Dummy<IBlock>();
        block.ForwardQueue.Enqueue(normalNode);

        A.CallTo(() => parsingContext.DismissNodes).Returns(new[] {dismissNode});
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => rootBlockParser.ParseAsync(A<XmlReader>.Ignored, parsingContext)).Returns(block);

        var result = (Storyboard)await sut.ParseAsync(reader);

        result.Should().NotBeNull();
        result.NavigationContext.RootBlock.Should().BeSameAs(block);
        block.ForwardQueue.Dequeue().Should().BeSameAs(normalNode);
        block.ForwardQueue.Dequeue().Should().BeSameAs(dismissNode);
        block.ForwardQueue.Count.Should().Be(0);
    }

    [Fact]
    public async Task DismissNodesShouldBeReversed()
    {
        var reader = new StringReader("<storyboard><abc /></storyboard>");

        var normalNode = A.Dummy<INode>();
        var dismissNode1 = A.Dummy<INode>();
        var dismissNode2 = A.Dummy<INode>();

        var block = A.Dummy<IBlock>();
        block.ForwardQueue.Enqueue(normalNode);

        A.CallTo(() => parsingContext.DismissNodes).Returns(new[] { dismissNode1, dismissNode2 });
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => rootBlockParser.ParseAsync(A<XmlReader>.Ignored, parsingContext)).Returns(block);

        var result = (Storyboard)await sut.ParseAsync(reader);

        block.ForwardQueue.Dequeue().Should().BeSameAs(normalNode);
        block.ForwardQueue.Dequeue().Should().BeSameAs(dismissNode2);
        block.ForwardQueue.Dequeue().Should().BeSameAs(dismissNode1);
        block.ForwardQueue.Count.Should().Be(0);
    }
}
