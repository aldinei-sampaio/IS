using System.Xml;

namespace IS.Reading.Parsing.Text;

public class BackgroundImageTextParser : ITextParser
{
    public static BackgroundImageTextParser Instance = new();

    private BackgroundImageTextParser()
    {
    }

    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim();
    }
}
