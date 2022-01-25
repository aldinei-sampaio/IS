namespace IS.Reading.Variables;

public struct TextSource : ITextSource
{
    public IInterpolator? Parsed { get; } = null;

    public string? Text { get; } = null;

    public TextSource(IInterpolator parsed)
        => Parsed = parsed;
    
    public TextSource(string text)
        => Text = text;

    public string ToString(IVariableDictionary variables)
    {
        if (Text is not null)
            return Text;

        if (Parsed is not null)
            return Parsed.Interpolate(variables);

        throw new InvalidOperationException("Estrutura não inicializada.");
    }
}
