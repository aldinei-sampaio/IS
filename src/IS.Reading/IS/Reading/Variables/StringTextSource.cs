namespace IS.Reading.Variables;

public struct StringTextSource : ITextSource
{
    public StringTextSource(string text)
        => Text = text;

    public string Text { get; }

    public string Build(IVariableDictionary variables)
        => Text;

    public override string ToString() 
        => Text;
}