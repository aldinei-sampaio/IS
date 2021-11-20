using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.Text;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class BackgroundColorNodeParser : NodeParserBase
{
    public static BackgroundColorNodeParser Instance = new();

    private BackgroundColorNodeParser()
    {
        TextParser = ColorTextParser.Instance;
    }

    public override string ElementName => "color";

    protected override INode? CreateNode(XmlReader reader, IParsingContext parsingContext, ParsedData parsed)
    { 
        if (string.IsNullOrEmpty(parsed.Text))
        {
            parsingContext.LogError(reader, "Era esperado o nome da cor.");
            return null;
        }
        return new BackgroundColorNode(parsed.Text, parsed.When);
    }
}
