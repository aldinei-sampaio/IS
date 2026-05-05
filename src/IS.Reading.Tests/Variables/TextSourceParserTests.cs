namespace IS.Reading.Variables;

public class TextSourceParserTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("Hello, World!", "Hello, World!")]
    [InlineData("{}", "")]
    [InlineData("{{abc}}", "{abc}")]
    public void ShouldReturnStringTextSourceWhenThereIsNoVariables(string argument, string expected)
    {
        var sut = new TextSourceParser();
        var result = sut.Parse(argument);
        result.IsOk.Should().BeTrue();
        result.Value.Should().BeOfType<StringTextSource>()
            .Which.Text.Should().Be(expected);
    }

    [Theory]
    [InlineData("{a}", "a")]
    [InlineData("{alpha}", "alpha")]
    [InlineData("{beta}", "beta")]
    public void ShouldReturnVariableTextSourceWhenThereIsASingleVariableAndNoText(string argument, string expected)
    {
        var sut = new TextSourceParser();
        var result = sut.Parse(argument);
        result.IsOk.Should().BeTrue();
        result.Value.Should().BeOfType<VariableTextSource>()
            .Which.VariableName.Should().Be(expected);
    }

    [Theory]
    [InlineData("Prezado sr. {Nome}")]
    [InlineData("{Area} {DDD} {Fone}")]
    [InlineData("{{IstoNaoEhVariavel}} {IstoEhVariavel}")]
    public void ShouldReturnInterpolatedTextSourceWhenThereIsVariablesAndText(string text)
    {
        var sut = new TextSourceParser();
        var result = sut.Parse(text);
        result.IsOk.Should().BeTrue();
        result.Value.Should().BeOfType<InterpolatedTextSource>()
            .Which.ToString().Should().Be(text);
    }

    [Theory]
    [InlineData("{", TextSourceParser.MissingClosingCurlyBraces)]
    [InlineData("abc{def", TextSourceParser.MissingClosingCurlyBraces)]
    [InlineData("abc{def}ghi{", TextSourceParser.MissingClosingCurlyBraces)]
    [InlineData("abc{def}ghi{jkl", TextSourceParser.MissingClosingCurlyBraces)]
    [InlineData("}", TextSourceParser.MissingOpeningCurlyBraces)]
    [InlineData("abc}", TextSourceParser.MissingOpeningCurlyBraces)]
    [InlineData("abc}def", TextSourceParser.MissingOpeningCurlyBraces)]
    [InlineData("abc{def}ghi}", TextSourceParser.MissingOpeningCurlyBraces)]
    [InlineData("abc{def}ghi}jkl", TextSourceParser.MissingOpeningCurlyBraces)]
    [InlineData("{0}", "Nome de variável inválido: '0'")]
    [InlineData("{0000}", "Nome de variável inválido: '0000'")]
    [InlineData("{ abc }", "Nome de variável inválido: ' abc '")]
    public void Failure(string text, string errorMessage)
    {
        var sut = new TextSourceParser();
        var result = sut.Parse(text);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void TwoVariables()
    {
        var text = "{MC} empurrou {antagonist} da ponte.";
        var sut = new TextSourceParser();
        var result = sut.Parse(text);
        result.IsOk.Should().BeTrue();

        result.Value.Should().BeOfType<InterpolatedTextSource>()
            .Which.Values.Should().BeEquivalentTo(new[]
            {
                new { IsVariable = true, Value = "MC" },
                new { IsVariable = false, Value = " empurrou " },
                new { IsVariable = true, Value = "antagonist" },
                new { IsVariable = false, Value = " da ponte." }
            });
    }
}
