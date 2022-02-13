namespace IS.Reading.Parsing;

public class ElementParserTests
{
    private readonly IElementParserSettings settings;
    private readonly IParserDictionary<INodeParser> childParsers;
    private readonly IDocumentReader reader;
    private readonly IParsingContext context;
    private readonly IParentParsingContext parentContext;
    private readonly ElementParser sut;

    public ElementParserTests()
    {
        settings = A.Dummy<IElementParserSettings>();
        childParsers = A.Fake<IParserDictionary<INodeParser>>(i => i.Strict());
        A.CallTo(() => settings.ChildParsers).Returns(childParsers);

        reader = A.Fake<IDocumentReader>(i => i.Strict());
        context = A.Fake<IParsingContext>(i => i.Strict());
        parentContext = A.Fake<IParentParsingContext>(i => i.Strict());
        sut = new ElementParser();
    }

    [Fact]
    public async Task Empty()
    {
        A.CallTo(() => reader.ReadAsync()).Returns(false);
        await sut.ParseAsync(reader, context, parentContext, settings);
        A.CallTo(() => reader.ReadAsync()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("alpha")]
    [InlineData("end")]
    [InlineData("else")]
    public async Task CommandNotRecognized(string command)
    {
        var errorMessage = $"Comando não reconhecido: '{command}'.";

        A.CallTo(() => reader.ReadAsync()).Returns(true);
        A.CallTo(() => reader.ElementName).Returns(command);
        A.CallTo(() => childParsers[command]).Returns(null);
        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext, settings);

        A.CallTo(() => reader.ReadAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(() => childParsers[command]).MustHaveHappenedOnceExactly();
        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("else")]
    [InlineData("ELSE")]
    [InlineData("Else")]
    public async Task ExitOnElse(string command)
    {
        A.CallTo(() => settings.ExitOnElse).Returns(true);

        A.CallTo(() => reader.ReadAsync()).Returns(true);
        A.CallTo(() => reader.ElementName).Returns(command);

        await sut.ParseAsync(reader, context, parentContext, settings);

        A.CallTo(() => reader.ReadAsync()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("end")]
    [InlineData("END")]
    [InlineData("End")]
    public async Task ExitOnEnd(string command)
    {
        A.CallTo(() => settings.ExitOnEnd).Returns(true);

        A.CallTo(() => reader.ReadAsync()).Returns(true);
        A.CallTo(() => reader.ElementName).Returns(command);

        await sut.ParseAsync(reader, context, parentContext, settings);

        A.CallTo(() => reader.ReadAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExitOnUnknownNode()
    {
        A.CallTo(() => settings.ExitOnUnknownNode).Returns(true);

        var command = "alpha";

        A.CallTo(() => reader.ReadAsync()).Returns(true);
        A.CallTo(() => reader.ElementName).Returns(command);
        A.CallTo(() => childParsers[command]).Returns(null);

        await sut.ParseAsync(reader, context, parentContext, settings);

        A.CallTo(() => reader.ReadAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task RequiredArgument()
    {
        var command = "omega";

        var errorMessage = $"O comando '{command}' requer um argumento.";
        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();

        var parser = A.Fake<INodeParser>(i => i.Strict());
        A.CallTo(() => parser.Name).Returns(command);
        A.CallTo(() => parser.IsArgumentRequired).Returns(true);
        A.CallTo(() => childParsers[command]).Returns(parser);

        A.CallTo(() => reader.ReadAsync()).Returns(true);
        A.CallTo(() => reader.ElementName).Returns(command);
        A.CallTo(() => reader.Argument).Returns(string.Empty);

        await sut.ParseAsync(reader, context, parentContext, settings);

        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
        A.CallTo(() => reader.ReadAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task NoRepeatNode()
    {
        A.CallTo(() => settings.NoRepeatNode).Returns(true);

        var command = "beta";

        var parser = A.Fake<INodeParser>(i => i.Strict(StrictFakeOptions.AllowObjectMethods));
        A.CallTo(() => parser.Name).Returns(command);
        A.CallTo(() => parser.IsArgumentRequired).Returns(false);
        A.CallTo(() => parser.ParseAsync(reader, context, parentContext)).DoesNothing();
        A.CallTo(() => childParsers[command]).Returns(parser);

        A.CallTo(() => reader.ReadAsync()).ReturnsNextFromSequence(true, true);
        A.CallTo(() => reader.AtEnd).ReturnsNextFromSequence(true);
        A.CallTo(() => reader.ElementName).Returns(command);

        await sut.ParseAsync(reader, context, parentContext, settings);

        A.CallTo(() => reader.ReadAsync()).MustHaveHappenedTwiceExactly();
        A.CallTo(() => parser.ParseAsync(reader, context, parentContext)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Aggregation()
    {
        var command = "gamma";
        var parser = A.Fake<IAggregateNodeParser>(i => i.Strict(StrictFakeOptions.AllowObjectMethods));
        A.CallTo(() => parser.Name).Returns(command);
        A.CallTo(() => parser.IsArgumentRequired).Returns(false);
        A.CallTo(() => parser.ParseAsync(reader, context, parentContext)).DoesNothing();
        A.CallTo(() => childParsers[command]).Returns(parser);

        A.CallTo(() => reader.ReadAsync()).ReturnsNextFromSequence(true);
        A.CallTo(() => reader.AtEnd).ReturnsNextFromSequence(true);
        A.CallTo(() => reader.ElementName).Returns(command);

        await sut.ParseAsync(reader, context, parentContext, settings);

        A.CallTo(() => reader.ReadAsync()).MustHaveHappenedOnceExactly();
        A.CallTo(() => parser.ParseAsync(reader, context, parentContext)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task TwoParsers()
    {
        var command1 = "alpha";
        var parser1 = CreateParser(command1);
        var command2 = "beta";
        var parser2 = CreateParser(command2);

        var currentCommand = string.Empty;

        A.CallTo(() => reader.ReadAsync())
            .Invokes(i => SwitchCurrentCommand())
            .ReturnsNextFromSequence(true, true, false);
        A.CallTo(() => reader.AtEnd).ReturnsNextFromSequence(false, true);
        A.CallTo(() => reader.ElementName).ReturnsLazily(i => currentCommand);

        await sut.ParseAsync(reader, context, parentContext, settings);

        A.CallTo(() => reader.ReadAsync()).MustHaveHappened(3, Times.Exactly);
        A.CallTo(() => parser1.ParseAsync(reader, context, parentContext)).MustHaveHappenedOnceExactly();
        A.CallTo(() => parser2.ParseAsync(reader, context, parentContext)).MustHaveHappenedOnceExactly();

        void SwitchCurrentCommand()
        {
            if (currentCommand == string.Empty)
                currentCommand = command1;
            else if (currentCommand == command1)
                currentCommand = command2;
        }

        INodeParser CreateParser(string command)
        {
            var parser = A.Fake<INodeParser>(i => i.Strict(StrictFakeOptions.AllowObjectMethods));
            A.CallTo(() => parser.Name).Returns(command);
            A.CallTo(() => parser.IsArgumentRequired).Returns(false);
            A.CallTo(() => parser.ParseAsync(reader, context, parentContext)).DoesNothing();
            A.CallTo(() => childParsers[command]).Returns(parser);
            return parser;
        }
    }
}

