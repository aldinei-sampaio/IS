using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.Text;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class BackgroundRightNodeParser : NodeParserBase
{
    public static BackgroundRightNodeParser Instance = new();

    private BackgroundRightNodeParser()
    {
        TextParser = BackgroundImageTextParser.Instance;
    }

    public override string ElementName => "right";

    protected override INode? CreateNode(XmlReader reader, IParsingContext parsingContext, ParsedData parsed)
    {
        if (string.IsNullOrEmpty(parsed.Text))
        {
            parsingContext.LogError(reader, "Era esperado o nome da imagem.");
            return null;
        }
        return new BackgroundRightNode(parsed.Text, parsed.When);
    }
}
