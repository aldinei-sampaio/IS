using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.Text;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class BackgroundLeftNodeParser : NodeParserBase
{
    public static BackgroundLeftNodeParser Instance = new();

    private BackgroundLeftNodeParser()
    {
        TextParser = BackgroundImageTextParser.Instance;
    }

    public override string ElementName => "left";

    protected override INode? CreateNode(XmlReader reader, IParsingContext parsingContext, ParsedData parsed)
    {
        if (string.IsNullOrEmpty(parsed.Text))
        {
            parsingContext.LogError(reader, "Era esperado o nome da imagem.");
            return null;
        }
        return new BackgroundLeftNode(parsed.Text, parsed.When);
    }
}
