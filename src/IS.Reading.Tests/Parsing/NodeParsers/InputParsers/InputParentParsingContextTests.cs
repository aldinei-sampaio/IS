using IS.Reading.Nodes;

namespace IS.Reading.Parsing.NodeParsers.InputParsers;

public class InputParentParsingContextTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new InputParentParsingContext("rho");
        sut.InputBuilder.TitleSource.Should().BeNull();
        sut.InputBuilder.TextSource.Should().BeNull();
        sut.InputBuilder.MaxLength.Should().Be(InputBuilder.MaxLenghtDefaultValue);
        sut.InputBuilder.ConfirmationSource.Should().BeNull();
    }
}
