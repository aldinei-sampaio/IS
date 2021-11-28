using System.Xml;

namespace IS.Reading.Parsing.TextParsers;

public class BalloonTextParser : IBalloonTextParser
{
    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // TODO: Replace character names and other variables

        return value;
    }
}
