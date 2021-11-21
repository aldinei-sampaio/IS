using IS.Reading.Navigation;
using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class PauseNodeParser : NodeParserBase
{
    public static PauseNodeParser Instance = new();

    private PauseNodeParser()
    {
    }

    public override string ElementName => "pause";

    protected override INode? CreateNode(XmlReader reader, IParsingContext parsingContext, ElementParsedData parsed)
        => new PauseNode(parsed.When);
}
