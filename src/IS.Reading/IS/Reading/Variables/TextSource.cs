namespace IS.Reading.Variables;

public struct TextSource : ITextSource
{
    public IInterpolator? Interpolator { get; } = null;

    public string? Text { get; } = null;

    public TextSource(IInterpolator interpolator)
        => Interpolator = interpolator;
    
    public TextSource(string text)
        => Text = text;

    public string Build(IVariableDictionary variables)
    {
        if (Text is not null)
            return Text;

        if (Interpolator is not null)
            return Interpolator.ToString(variables);

        throw new InvalidOperationException("Estrutura não inicializada.");
    }

    public override string? ToString()
    {
        if (Text is not null)
            return Text;

        if (Interpolator is not null)
            return Interpolator.ToString();

        throw new InvalidOperationException("Estrutura não inicializada.");
    }
}
