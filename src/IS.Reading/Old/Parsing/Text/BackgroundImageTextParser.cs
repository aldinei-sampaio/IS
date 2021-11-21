using System.Xml;

namespace IS.Reading.Parsing.Text;

public class BackgroundImageTextParser : IBackgroundImageTextParser
{
    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.Trim();
    }
}
