using IS.Reading.Variables;
using System.Text;
using System.Xml;

namespace IS.Reading.Parsing.ArgumentParsers;

public class BalloonTextParser : IBalloonTextParser
{
    public string? Parse(XmlReader reader, IParsingContext parsingContext, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value;
    }
}
