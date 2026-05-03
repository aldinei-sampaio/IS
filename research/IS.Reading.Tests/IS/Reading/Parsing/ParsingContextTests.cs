using IS.Reading.Navigation;
using System.Text;

namespace IS.Reading.Parsing;

public class ParsingContextTests
{
    private readonly IBlockFactory blockFactory;
    private readonly IParsingSceneContext parsingSceneContext;
    private readonly ParsingContext sut;

    public ParsingContextTests()
    {
        blockFactory = A.Dummy<IBlockFactory>();
        parsingSceneContext = A.Dummy<ParsingSceneContext>();
        sut = new(blockFactory, parsingSceneContext);
    }

    [Fact]
    public void Initialization()
    {
        sut.BlockFactory.Should().BeSameAs(blockFactory);
        sut.SceneContext.Should().BeSameAs(parsingSceneContext);
        sut.IsSuccess.Should().BeTrue();
        sut.DismissNodes.Should().BeEmpty();
    }

    [Fact]
    public void IsSuccessShouldReturnFalseAfterLogError()
    {
        var reader = A.Fake<IDocumentReader>(i => i.Strict());
        A.CallTo(() => reader.CurrentLineIndex).Returns(1);

        sut.IsSuccess.Should().BeTrue();
        sut.LogError(reader, "Erro");

        sut.IsSuccess.Should().BeFalse();
        sut.ToString().Should().Be("Linha 1: Erro");
    }

    [Fact]
    public void LineNumberInErrorMessages()
    {
        var reader = A.Fake<IDocumentReader>(i => i.Strict());
        A.CallTo(() => reader.CurrentLineIndex).ReturnsNextFromSequence(2, 3);

        sut.LogError(reader, "b");
        sut.LogError(reader, "c");

        sut.IsSuccess.Should().BeFalse();
        sut.ToString().Should().Be($"Linha 2: b{Environment.NewLine}Linha 3: c");
    }

    [Fact]
    public void MaxNumberOfErrorsExceeded()
    {
        var reader = A.Fake<IDocumentReader>(i => i.Strict());
        A.CallTo(() => reader.CurrentLineIndex).Returns(1);

        sut.IsSuccess.Should().BeTrue();

        for (var n = 1; n <= 11; n++)
            sut.LogError(reader, "Erro");

        sut.IsSuccess.Should().BeFalse();

        var builder = new StringBuilder();
        for (var n = 1; n <= 10; n++)
            builder.AppendLine("Linha 1: Erro");
        builder.Append("Número máximo de erros atingido.");

        var expected = builder.ToString();
        sut.ToString().Should().Be(expected);
    }

    [Fact]
    public void DismissNodes()
    {
        sut.DismissNodes.Should().BeEmpty();

        var node = A.Dummy<INode>();
        sut.RegisterDismissNode(node);

        sut.DismissNodes.Should().ContainSingle().Which.Should().BeSameAs(node);
    }
}
