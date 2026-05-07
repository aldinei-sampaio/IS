using IS.Reading.State;

namespace IS.Reading.Parsing.ArgumentParsers;

public class BackgroundColorArgumentParserTests
{
    private readonly IColorArgumentParser colorArgumentParser;
    private readonly BackgroundColorArgumentParser sut;

    public BackgroundColorArgumentParserTests()
    {
        colorArgumentParser = A.Fake<IColorArgumentParser>(i => i.Strict());
        sut = new(colorArgumentParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.ColorArgumentParser.Should().BeSameAs(colorArgumentParser);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldFailForEmptyArgument(string? value)
    {
        var result = sut.Parse(value!);
        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be("Era esperado um argumento com a cor.");
    }

    [Fact]
    public void ShouldPropagateColorParserError()
    {
        var errorMessage = "Cor inválida.";
        A.CallTo(() => colorArgumentParser.Parse("salmon")).Returns(Result.Fail<string>(errorMessage));

        var result = sut.Parse("salmon");

        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void ShouldParseColorOnly()
    {
        A.CallTo(() => colorArgumentParser.Parse("black")).Returns(Result.Ok("black"));

        var result = sut.Parse("black");

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(new BackgroundColorArgument("black", BackgroundAnimation.None, null));
    }

    [Fact]
    public void ShouldParseColorWithExtraWhitespace()
    {
        A.CallTo(() => colorArgumentParser.Parse("black")).Returns(Result.Ok("black"));

        var result = sut.Parse("  black  ");

        result.IsOk.Should().BeTrue();
        result.Value.ColorValue.Should().Be("black");
        result.Value.Animation.Should().Be(BackgroundAnimation.None);
    }

    [Theory]
    [InlineData("fadein",   BackgroundAnimation.FadeIn)]
    [InlineData("FADEIN",   BackgroundAnimation.FadeIn)]
    [InlineData("FadeIn",   BackgroundAnimation.FadeIn)]
    [InlineData("zoom",     BackgroundAnimation.Zoom)]
    [InlineData("ZOOM",     BackgroundAnimation.Zoom)]
    [InlineData("dissolve", BackgroundAnimation.Dissolve)]
    [InlineData("DISSOLVE", BackgroundAnimation.Dissolve)]
    public void ShouldParseAnimationKeyword(string keyword, BackgroundAnimation expectedAnimation)
    {
        A.CallTo(() => colorArgumentParser.Parse("black")).Returns(Result.Ok("black"));

        var result = sut.Parse($"black {keyword}");

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(new BackgroundColorArgument("black", expectedAnimation, null));
    }

    [Fact]
    public void ShouldParseFlashWithoutColor()
    {
        A.CallTo(() => colorArgumentParser.Parse("black")).Returns(Result.Ok("black"));

        var result = sut.Parse("black flash");

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(new BackgroundColorArgument("black", BackgroundAnimation.Flash, null));
    }

    [Fact]
    public void ShouldParseFlashWithColor()
    {
        A.CallTo(() => colorArgumentParser.Parse("black")).Returns(Result.Ok("black"));
        A.CallTo(() => colorArgumentParser.Parse("white")).Returns(Result.Ok("white"));

        var result = sut.Parse("black flash white");

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(new BackgroundColorArgument("black", BackgroundAnimation.Flash, "white"));
    }

    [Fact]
    public void ShouldParseFlashWithHexColor()
    {
        A.CallTo(() => colorArgumentParser.Parse("black")).Returns(Result.Ok("black"));
        A.CallTo(() => colorArgumentParser.Parse("#ff0000")).Returns(Result.Ok("#ff0000"));

        var result = sut.Parse("black flash #ff0000");

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(new BackgroundColorArgument("black", BackgroundAnimation.Flash, "#ff0000"));
    }

    [Fact]
    public void ShouldFailForInvalidAnimationKeyword()
    {
        A.CallTo(() => colorArgumentParser.Parse("black")).Returns(Result.Ok("black"));

        var result = sut.Parse("black blink");

        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be("O texto 'blink' não é uma animação válida. As animações disponíveis são: fadein, zoom, dissolve, flash.");
    }

    [Theory]
    [InlineData("fadein")]
    [InlineData("zoom")]
    [InlineData("dissolve")]
    public void ShouldFailWhenFlashColorGivenForNonFlashAnimation(string animation)
    {
        A.CallTo(() => colorArgumentParser.Parse("black")).Returns(Result.Ok("black"));

        var result = sut.Parse($"black {animation} white");

        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be("A cor do flash só pode ser especificada para a animação 'flash'.");
    }

    [Fact]
    public void ShouldFailForTooManyArguments()
    {
        A.CallTo(() => colorArgumentParser.Parse("black")).Returns(Result.Ok("black"));

        var result = sut.Parse("black flash white extra");

        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be("Muitos argumentos para o comando 'color'.");
    }

    [Fact]
    public void ShouldPropagateFlashColorParserError()
    {
        var errorMessage = "Cor inválida.";
        A.CallTo(() => colorArgumentParser.Parse("black")).Returns(Result.Ok("black"));
        A.CallTo(() => colorArgumentParser.Parse("salmon")).Returns(Result.Fail<string>(errorMessage));

        var result = sut.Parse("black flash salmon");

        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }
}
