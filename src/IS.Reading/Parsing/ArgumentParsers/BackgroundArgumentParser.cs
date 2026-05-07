using IS.Reading.State;

namespace IS.Reading.Parsing.ArgumentParsers;

public record BackgroundArgument(string ImageName, BackgroundAnimation Animation, string? FlashColor);

public interface IBackgroundArgumentParser
{
    Result<BackgroundArgument> Parse(string value);
}

public class BackgroundArgumentParser(
    IImageArgumentParser imageArgumentParser,
    IColorArgumentParser colorArgumentParser
) : IBackgroundArgumentParser
{
    public IImageArgumentParser ImageArgumentParser { get; } = imageArgumentParser;
    public IColorArgumentParser ColorArgumentParser { get; } = colorArgumentParser;

    public Result<BackgroundArgument> Parse(string value) => Parse(value.AsSpan());

    private Result<BackgroundArgument> Parse(ReadOnlySpan<char> value)
    {
        value = value.Trim();
        if (value.IsEmpty)
            return Result.Fail<BackgroundArgument>("Era esperado um argumento com o nome da imagem.");

        Span<Range> parts = stackalloc Range[4];
        var count = value.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

        var imageResult = ImageArgumentParser.Parse(value[parts[0]].ToString());
        if (!imageResult.IsOk)
            return Result.Fail<BackgroundArgument>(imageResult.ErrorMessage);

        if (count == 1)
            return Result.Ok(new BackgroundArgument(imageResult.Value, BackgroundAnimation.None, null));

        if (count > 3)
            return Result.Fail<BackgroundArgument>("Muitos argumentos para o comando 'background'.");

        var keyword = value[parts[1]];
        var animation = GetAnimation(keyword);

        if (animation is null)
            return Result.Fail<BackgroundArgument>($"O texto '{keyword}' não é uma animação válida. As animações disponíveis são: fadein, zoom, dissolve, flash.");

        if (animation != BackgroundAnimation.Flash)
        {
            if (count > 2)
                return Result.Fail<BackgroundArgument>("A cor do flash só pode ser especificada para a animação 'flash'.");
            return Result.Ok(new BackgroundArgument(imageResult.Value, animation.Value, null));
        }

        if (count == 2)
            return Result.Ok(new BackgroundArgument(imageResult.Value, BackgroundAnimation.Flash, null));

        var colorResult = ColorArgumentParser.Parse(value[parts[2]].ToString());
        if (!colorResult.IsOk)
            return Result.Fail<BackgroundArgument>(colorResult.ErrorMessage);

        return Result.Ok(new BackgroundArgument(imageResult.Value, BackgroundAnimation.Flash, colorResult.Value));
    }

    private static BackgroundAnimation? GetAnimation(ReadOnlySpan<char> keyword)
    {
        if (keyword.Equals("fadein",   StringComparison.OrdinalIgnoreCase)) return BackgroundAnimation.FadeIn;
        if (keyword.Equals("zoom",     StringComparison.OrdinalIgnoreCase)) return BackgroundAnimation.Zoom;
        if (keyword.Equals("dissolve", StringComparison.OrdinalIgnoreCase)) return BackgroundAnimation.Dissolve;
        if (keyword.Equals("flash",    StringComparison.OrdinalIgnoreCase)) return BackgroundAnimation.Flash;
        return null;
    }
}
