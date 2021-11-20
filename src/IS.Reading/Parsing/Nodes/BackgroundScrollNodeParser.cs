using IS.Reading.Navigation;
using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class BackgroundScrollNodeParser : NodeParserBase
{
    public static BackgroundScrollNodeParser Instance = new();

    private BackgroundScrollNodeParser()
    {
    }

    public override string ElementName => "scroll";

    protected override INode? CreateNode(XmlReader reader, IParsingContext parsingContext, ParsedData parsed)
        => new BackgroundScrollNode(parsed.When);
}
