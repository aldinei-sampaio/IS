using System.Text;

namespace IS.Reading.Variables;

public struct StringTextSource : ITextSource
{
    public StringTextSource(string text)
        => Text = text;

    public string Text { get; }

    public string Build(IVariableDictionary variables)
        => Text;

    public override string ToString() 
    {
        if (Text.IndexOfAny(new[] {'{', '}'}) < 0)
            return Text;

        var builder = new StringBuilder(Text.Length);
        foreach(var c in Text)
        {
            builder.Append(c);
            if (c == '{' || c == '}')
                builder.Append(c);
        }
        return builder.ToString();
    }
}