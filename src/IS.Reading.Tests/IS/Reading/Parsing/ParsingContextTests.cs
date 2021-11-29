using System.Text;
using System.Xml;

namespace IS.Reading.Parsing;

public class ParsingContextTests
{
    [Fact]
    public void Person()
    {
        var sut = new ParsingContext();
        sut.Person.Should().BeNull();
        sut.Person = "abc";
        sut.Person.Should().Be("abc");
    }

    [Fact]
    public void IsSuccessShouldReturnFalseAfterLogError()
    {
        var sut = new ParsingContext();
        sut.IsSuccess.Should().BeTrue();

        using var reader = XmlReader.Create(new StringReader("<teste />"));
        reader.MoveToContent();
        sut.LogError(reader, "Erro");

        sut.IsSuccess.Should().BeFalse();
        sut.ToString().Should().Be("Linha 1: Erro");
    }

    [Fact]
    public void LineNumberInErrorMessages()
    {
        var xml = "<a>\r\n<b />\r\n<c /></a>";
        using var reader = XmlReader.Create(new StringReader(xml));
        reader.MoveToContent();

        var sut = new ParsingContext();

        while(reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
                sut.LogError(reader, reader.LocalName);
        }

        sut.IsSuccess.Should().BeFalse();
        sut.ToString().Should().Be($"Linha 2: b{Environment.NewLine}Linha 3: c");
    }

    [Fact]
    public void MaxNumberOfErrorsExceeded()
    {
        var sut = new ParsingContext();
        sut.IsSuccess.Should().BeTrue();

        using var reader = XmlReader.Create(new StringReader("<teste />"));
        reader.MoveToContent();

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
}
