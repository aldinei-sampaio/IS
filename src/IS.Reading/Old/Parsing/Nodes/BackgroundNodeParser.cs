using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.Attributes;
using IS.Reading.Parsing.Text;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class BackgroundNodeParser : NodeParserBase
{
    public static BackgroundNodeParser Instance = new();

    private BackgroundNodeParser()
    {
        TextParser = BackgroundImageTextParser.Instance;
        AttributeParsers.Add(WhenAttributeParser.Instance);
        ChildParsers.Add(BackgroundColorNodeParser.Instance);
        ChildParsers.Add(BackgroundLeftNodeParser.Instance);
        ChildParsers.Add(BackgroundRightNodeParser.Instance);
        ChildParsers.Add(BackgroundScrollNodeParser.Instance);
        ChildParsers.Add(PauseNodeParser.Instance);
    }

    public override string ElementName => "background";

    protected override INode? CreateNode(XmlReader reader, IParsingContext parsingContext, ElementParsedData parsed)
    {
        if (!string.IsNullOrWhiteSpace(parsed.Text))
        {
            var block = new Block();
            block.Add(new BackgroundLeftNode(parsed.Text, null));
            block.Add(new BackgroundScrollNode(null));
            return new BlockNode(block, parsed.When, parsed.While);
        }

        if (parsed.Block is null || parsed.Block.IsEmpty())
        {
            parsingContext.LogError(reader, "Nome de imagem ou elemento filho era esperado.");
            return null;
        }

        return new BlockNode(parsed.Block, parsed.When, parsed.While);
    }
}
