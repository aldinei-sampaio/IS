using IS.Reading.State;

namespace IS.Reading.Parsing.ArgumentParsers;

public class BackgroundArgumentParserTests
{
    private readonly IImageArgumentParser imageArgumentParser;
    private readonly IColorArgumentParser colorArgumentParser;
    private readonly BackgroundArgumentParser sut;

    public BackgroundArgumentParserTests()
    {
        imageArgumentParser = A.Fake<IImageArgumentParser>(i => i.Strict());
        colorArgumentParser = A.Fake<IColorArgumentParser>(i => i.Strict());
        sut = new(imageArgumentParser, colorArgumentParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.ImageArgumentParser.Should().BeSameAs(imageArgumentParser);
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
        result.ErrorMessage.Should().Be("Era esperado um argumento com o nome da imagem.");
    }

    [Fact]
    public void ShouldPropagateImageParserError()
    {
        var errorMessage = "Imagem inválida.";
        A.CallTo(() => imageArgumentParser.Parse("???")).Returns(Result.Fail<string>(errorMessage));

        var result = sut.Parse("???");

        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void ShouldParseImageNameOnly()
    {
        A.CallTo(() => imageArgumentParser.Parse("forest")).Returns(Result.Ok("forest"));

        var result = sut.Parse("forest");

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(new BackgroundArgument("forest", BackgroundAnimation.None, null));
    }

    [Fact]
    public void ShouldParseImageNameWithExtraWhitespace()
    {
        A.CallTo(() => imageArgumentParser.Parse("forest")).Returns(Result.Ok("forest"));

        var result = sut.Parse("  forest  ");

        result.IsOk.Should().BeTrue();
        result.Value.ImageName.Should().Be("forest");
        result.Value.Animation.Should().Be(BackgroundAnimation.None);
    }

    [Theory]
    [InlineData("fadein",  BackgroundAnimation.FadeIn)]
    [InlineData("FADEIN",  BackgroundAnimation.FadeIn)]
    [InlineData("FadeIn",  BackgroundAnimation.FadeIn)]
    [InlineData("zoom",    BackgroundAnimation.Zoom)]
    [InlineData("ZOOM",    BackgroundAnimation.Zoom)]
    [InlineData("dissolve", BackgroundAnimation.Dissolve)]
    [InlineData("DISSOLVE", BackgroundAnimation.Dissolve)]
    public void ShouldParseAnimationKeyword(string keyword, BackgroundAnimation expectedAnimation)
    {
        A.CallTo(() => imageArgumentParser.Parse("forest")).Returns(Result.Ok("forest"));

        var result = sut.Parse($"forest {keyword}");

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(new BackgroundArgument("forest", expectedAnimation, null));
    }

    [Fact]
    public void ShouldParseFlashWithoutColor()
    {
        A.CallTo(() => imageArgumentParser.Parse("forest")).Returns(Result.Ok("forest"));

        var result = sut.Parse("forest flash");

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(new BackgroundArgument("forest", BackgroundAnimation.Flash, null));
    }

    [Fact]
    public void ShouldParseFlashWithColor()
    {
        A.CallTo(() => imageArgumentParser.Parse("forest")).Returns(Result.Ok("forest"));
        A.CallTo(() => colorArgumentParser.Parse("white")).Returns(Result.Ok("white"));

        var result = sut.Parse("forest flash white");

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(new BackgroundArgument("forest", BackgroundAnimation.Flash, "white"));
    }

    [Fact]
    public void ShouldParseFlashWithHexColor()
    {
        A.CallTo(() => imageArgumentParser.Parse("forest")).Returns(Result.Ok("forest"));
        A.CallTo(() => colorArgumentParser.Parse("#ff0000")).Returns(Result.Ok("#ff0000"));

        var result = sut.Parse("forest flash #ff0000");

        result.IsOk.Should().BeTrue();
        result.Value.Should().Be(new BackgroundArgument("forest", BackgroundAnimation.Flash, "#ff0000"));
    }

    [Fact]
    public void ShouldFailForInvalidAnimationKeyword()
    {
        A.CallTo(() => imageArgumentParser.Parse("forest")).Returns(Result.Ok("forest"));

        var result = sut.Parse("forest blink");

        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be("O texto 'blink' não é uma animação válida. As animações disponíveis são: fadein, zoom, dissolve, flash.");
    }

    [Theory]
    [InlineData("fadein")]
    [InlineData("zoom")]
    [InlineData("dissolve")]
    public void ShouldFailWhenFlashColorGivenForNonFlashAnimation(string animation)
    {
        A.CallTo(() => imageArgumentParser.Parse("forest")).Returns(Result.Ok("forest"));

        var result = sut.Parse($"forest {animation} white");

        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be("A cor do flash só pode ser especificada para a animação 'flash'.");
    }

    [Fact]
    public void ShouldFailForTooManyArguments()
    {
        A.CallTo(() => imageArgumentParser.Parse("forest")).Returns(Result.Ok("forest"));

        var result = sut.Parse("forest flash white extra");

        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be("Muitos argumentos para o comando 'background'.");
    }

    [Fact]
    public void ShouldPropagateColorParserError()
    {
        var errorMessage = "Cor inválida.";
        A.CallTo(() => imageArgumentParser.Parse("forest")).Returns(Result.Ok("forest"));
        A.CallTo(() => colorArgumentParser.Parse("salmon")).Returns(Result.Fail<string>(errorMessage));

        var result = sut.Parse("forest flash salmon");

        result.IsOk.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }
}
