using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;
using IS.Reading.Variables;

namespace IS.Reading.Parsing;

public class StoryboardParserTests
{
    private readonly IDocumentReader reader;
    private readonly List<INode> dismissNodes;
    private readonly IParsingContext parsingContext;
    private readonly IBlockFactory blockFactory;
    private readonly IRootBlockParser rootBlockParser;
    private readonly ISceneNavigator sceneNavigator;
    private readonly IEventManager eventManager;
    private readonly IRandomizer randomizer;
    private readonly INavigationState navigationState;
    private readonly IVariableDictionary variableDictionary;
    private readonly IBlockState blockState;
    private readonly StoryboardParser sut;

    public StoryboardParserTests()
    {
        reader = A.Dummy<IDocumentReader>();

        blockFactory = new FakeBlockFactory();
        dismissNodes = new();
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => parsingContext.RegisterDismissNode(A<INode>.Ignored))
            .Invokes(i => dismissNodes.Add(i.Arguments.Get<INode>(0)));
        A.CallTo(() => parsingContext.BlockFactory).Returns(blockFactory);

        rootBlockParser = A.Fake<IRootBlockParser>(i => i.Strict());
        sceneNavigator = A.Fake<ISceneNavigator>(i => i.Strict());
        eventManager = A.Fake<IEventManager>(i => i.Strict());
        randomizer = A.Fake<IRandomizer>(i => i.Strict());
        navigationState = A.Fake<INavigationState>(i => i.Strict());
        variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        blockState = A.Fake<IBlockState>(i => i.Strict());

        var serviceProvider = A.Fake<IServiceProvider>(i => i.Strict());
        A.CallTo(() => serviceProvider.GetService(typeof(IParsingContext))).Returns(parsingContext);
        A.CallTo(() => serviceProvider.GetService(typeof(IRootBlockParser))).Returns(rootBlockParser);
        A.CallTo(() => serviceProvider.GetService(typeof(ISceneNavigator))).Returns(sceneNavigator);
        A.CallTo(() => serviceProvider.GetService(typeof(IEventManager))).Returns(eventManager);
        A.CallTo(() => serviceProvider.GetService(typeof(IRandomizer))).Returns(randomizer);
        A.CallTo(() => serviceProvider.GetService(typeof(INavigationState))).Returns(navigationState);
        A.CallTo(() => serviceProvider.GetService(typeof(IVariableDictionary))).Returns(variableDictionary);
        A.CallTo(() => serviceProvider.GetService(typeof(IBlockState))).Returns(blockState);
        sut = new StoryboardParser(serviceProvider);
    }

    [Fact]
    public async Task SimpleParsing()
    {
        var nodes = new List<INode>();
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => parsingContext.DismissNodes).Returns(Enumerable.Empty<INode>());
        A.CallTo(() => rootBlockParser.ParseAsync(reader, parsingContext)).Returns(nodes);

        var result = await sut.ParseAsync(reader);

        result.Should().BeOfType<Storyboard>()
            .Which.ShouldSatisfy(i =>
            {
                i.Events.Should().BeSameAs(eventManager);
                i.EventManager.Should().BeSameAs(eventManager);
                i.SceneNavigator.Should().BeSameAs(sceneNavigator);
                i.NavigationContext.ShouldSatisfy(i =>
                {
                    i.Variables.Should().BeSameAs(variableDictionary);
                    i.State.Should().BeSameAs(navigationState);
                    i.Events.Should().BeSameAs(eventManager);
                    i.RootBlock.Nodes.Should().BeSameAs(nodes);
                    i.EnteredBlocks.Should().BeEmpty();
                    i.EnteredBlockStates.Should().BeEmpty();
                    i.CurrentBlock.Should().BeNull();
                    i.RootBlockState.Should().BeSameAs(blockState);
                    i.CurrentNode.Should().BeNull();
                    i.Randomizer.Should().BeSameAs(randomizer);
                    i.RootBlockState.Should().BeSameAs(blockState);
                });                
            });
    }

    [Fact]
    public async Task RootBlockNodes()
    {
        var nodes = new List<INode>();

        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => parsingContext.DismissNodes).Returns(Enumerable.Empty<INode>());
        A.CallTo(() => rootBlockParser.ParseAsync(reader, parsingContext)).Returns(nodes);

        var result = await sut.ParseAsync(reader);

        result.Should().BeOfType<Storyboard>()
            .Which.NavigationContext.RootBlock.Nodes.Should().BeSameAs(nodes);
    }

    [Fact]
    public async Task ContextArgument()
    {
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);
        A.CallTo(() => parsingContext.ToString()).Returns("Erro proposital");
        A.CallTo(() => rootBlockParser.ParseAsync(reader, parsingContext)).Returns(new List<INode>());

        var ex = await Assert.ThrowsAsync<ParsingException>(
            () => sut.ParseAsync(reader)
        );

        ex.Message.Should().Be("Erro proposital");
    }

    [Fact]
    public async Task DismissNodesShouldBeAppendedToEndOfTheStoryboard()
    {
        var normalNode = A.Dummy<INode>();
        var dismissNode = A.Dummy<INode>();

        var nodes = new List<INode> { normalNode };

        A.CallTo(() => parsingContext.DismissNodes).Returns(new[] {dismissNode});
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => rootBlockParser.ParseAsync(reader, parsingContext)).Returns(nodes);

        var result = await sut.ParseAsync(reader);

        result.Should().BeOfType<Storyboard>()
            .Which.NavigationContext.RootBlock.ShouldBeEquivalentTo(normalNode, dismissNode);
    }

    [Fact]
    public async Task DismissNodesShouldBeReversed()
    {
        var normalNode = A.Dummy<INode>();
        var dismissNode1 = A.Dummy<INode>();
        var dismissNode2 = A.Dummy<INode>();

        var nodes = new List<INode> { normalNode };

        A.CallTo(() => parsingContext.DismissNodes).Returns(new[] { dismissNode1, dismissNode2 });
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => rootBlockParser.ParseAsync(reader, parsingContext)).Returns(nodes);

        var result = await sut.ParseAsync(reader);

        result.Should().BeOfType<Storyboard>()
            .Which.NavigationContext.RootBlock.ShouldBeEquivalentTo(normalNode, dismissNode2, dismissNode1);
    }
}
