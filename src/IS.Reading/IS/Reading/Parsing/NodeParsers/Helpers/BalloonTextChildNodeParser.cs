using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.Helpers;

public class BalloonTextChildNodeParser : INodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public BalloonTextChildNodeParser(
        IElementParser elementParser,
        IBalloonTextParser balloonTextParser,
        BalloonType balloonType,
        string name
    )
    {
        this.elementParser = elementParser;
        Settings = new ElementParserSettings(balloonTextParser);
        BalloonType = balloonType;
        Name = name;
    }

    public BalloonType BalloonType { get; }

    public string Name { get; }

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);
        if (parsed.Text is null)
            return null;

        return new BalloonTextNode(parsed.Text, BalloonType);
    }
}
