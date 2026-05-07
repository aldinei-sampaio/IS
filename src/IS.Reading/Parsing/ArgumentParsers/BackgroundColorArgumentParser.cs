using IS.Reading.State;

namespace IS.Reading.Parsing.ArgumentParsers;

public record BackgroundColorArgument(string ColorValue, BackgroundAnimation Animation, string? FlashColor);

public interface IBackgroundColorArgumentParser
{
    Result<BackgroundColorArgument> Parse(string value);
}

public class BackgroundColorArgumentParser(IColorArgumentParser colorArgumentParser) : IBackgroundColorArgumentParser
{
    public IColorArgumentParser ColorArgumentParser { get; } = colorArgumentParser;

    public Result<BackgroundColorArgument> Parse(string value) => Parse(value.AsSpan());

    private Result<BackgroundColorArgument> Parse(ReadOnlySpan<char> value)
    {
        value = value.Trim();
        if (value.IsEmpty)
            return Result.Fail<BackgroundColorArgument>("Era esperado um argumento com a cor.");

        Span<Range> parts = stackalloc Range[4];
        var count = value.Split(parts, ' ', StringSplitOptions.RemoveEmptyEntries);

        var colorResult = ColorArgumentParser.Parse(value[parts[0]].ToString());
        if (!colorResult.IsOk)
            return Result.Fail<BackgroundColorArgument>(colorResult.ErrorMessage);

        if (count == 1)
            return Result.Ok(new BackgroundColorArgument(colorResult.Value, BackgroundAnimation.None, null));

        if (count > 3)
            return Result.Fail<BackgroundColorArgument>("Muitos argumentos para o comando 'color'.");

        var keyword = value[parts[1]];
        var animation = GetAnimation(keyword);

        if (animation is null)
            return Result.Fail<BackgroundColorArgument>($"O texto '{keyword}' não é uma animação válida. As animações disponíveis são: fadein, zoom, dissolve, flash.");

        if (animation != BackgroundAnimation.Flash)
        {
            if (count > 2)
                return Result.Fail<BackgroundColorArgument>("A cor do flash só pode ser especificada para a animação 'flash'.");
            return Result.Ok(new BackgroundColorArgument(colorResult.Value, animation.Value, null));
        }

        if (count == 2)
            return Result.Ok(new BackgroundColorArgument(colorResult.Value, BackgroundAnimation.Flash, null));

        var flashColorResult = ColorArgumentParser.Parse(value[parts[2]].ToString());
        if (!flashColorResult.IsOk)
            return Result.Fail<BackgroundColorArgument>(flashColorResult.ErrorMessage);

        return Result.Ok(new BackgroundColorArgument(colorResult.Value, BackgroundAnimation.Flash, flashColorResult.Value));
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
