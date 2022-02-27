namespace IS.Reading.Parsing.NodeParsers;

public class ElementParserSettingsFactoryTests
{
    private readonly IMusicNodeParser musicNodeParser;
    private readonly IBackgroundNodeParser backgroundNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly IMainCharacterNodeParser mainCharacterNodeParser;
    private readonly IPersonNodeParser personNodeParser;
    private readonly INarrationNodeParser narrationNodeParser;
    private readonly ITutorialNodeParser tutorialNodeParser;
    private readonly ISetNodeParser setNodeParser;
    private readonly IInputNodeParser inputNodeParser;
    private readonly IIfNodeParser ifNodeParser;
    private readonly IWhileNodeParser whileNodeParser;
    private readonly IServiceProvider serviceProvider;
    private readonly ElementParserSettingsFactory sut;

    public ElementParserSettingsFactoryTests()
    {
        musicNodeParser = Helper.FakeParser<IMusicNodeParser>("music");
        backgroundNodeParser = Helper.FakeParser<IBackgroundNodeParser>("background");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");
        mainCharacterNodeParser = Helper.FakeParser<IMainCharacterNodeParser>("mc");
        personNodeParser = Helper.FakeParser<IPersonNodeParser>("person");
        narrationNodeParser = Helper.FakeParser<INarrationNodeParser>("narration");
        tutorialNodeParser = Helper.FakeParser<ITutorialNodeParser>("tutorial");
        setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        inputNodeParser = Helper.FakeParser<IInputNodeParser>("input");
        ifNodeParser = Helper.FakeParser<IIfNodeParser>("if");
        whileNodeParser = Helper.FakeParser<IWhileNodeParser>("while");

        serviceProvider = A.Fake<IServiceProvider>(i => i.Strict());
        A.CallTo(() => serviceProvider.GetService(typeof(IMusicNodeParser))).Returns(musicNodeParser);
        A.CallTo(() => serviceProvider.GetService(typeof(IBackgroundNodeParser))).Returns(backgroundNodeParser);
        A.CallTo(() => serviceProvider.GetService(typeof(IPauseNodeParser))).Returns(pauseNodeParser);
        A.CallTo(() => serviceProvider.GetService(typeof(IMainCharacterNodeParser))).Returns(mainCharacterNodeParser);
        A.CallTo(() => serviceProvider.GetService(typeof(IPersonNodeParser))).Returns(personNodeParser);
        A.CallTo(() => serviceProvider.GetService(typeof(INarrationNodeParser))).Returns(narrationNodeParser);
        A.CallTo(() => serviceProvider.GetService(typeof(ITutorialNodeParser))).Returns(tutorialNodeParser);
        A.CallTo(() => serviceProvider.GetService(typeof(ISetNodeParser))).Returns(setNodeParser);
        A.CallTo(() => serviceProvider.GetService(typeof(IInputNodeParser))).Returns(inputNodeParser);
        A.CallTo(() => serviceProvider.GetService(typeof(IIfNodeParser))).Returns(ifNodeParser);
        A.CallTo(() => serviceProvider.GetService(typeof(IWhileNodeParser))).Returns(whileNodeParser);

        sut = new(serviceProvider);
    }

    [Fact]
    public void Initialization()
    {
        var parsers = new INodeParser[]
        {
            musicNodeParser,
            backgroundNodeParser,
            pauseNodeParser,
            mainCharacterNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser,
            setNodeParser,
            inputNodeParser,
            ifNodeParser,
            whileNodeParser
        };

        sut.IfBlock.Should().BeOfType<ElementParserSettings.IfBlock>();
        sut.IfBlock.ChildParsers.Should().BeEquivalentTo(parsers);
        sut.Block.Should().BeOfType<ElementParserSettings.Block>();
        sut.Block.ChildParsers.Should().BeEquivalentTo(parsers);
        sut.NoBlock.Should().BeOfType<ElementParserSettings.NoBlock>();
        sut.NoBlock.ChildParsers.Should().BeEquivalentTo(parsers);
    }
}
