using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.Attributes;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class DoNodeParser : RootNodeParser
{
    public static new DoNodeParser Instance = new();

    private DoNodeParser() : base()
    {
        AttributeParsers.Add(WhenAttributeParser.Instance);
        AttributeParsers.Add(WhileAttributeParser.Instance);
    }

    public override string ElementName => "do";

    protected override INode? CreateNode(XmlReader reader, IParsingContext parsingContext, ParsedData parsed)
    {
        if (parsed.Block is null || parsed.Block.IsEmpty())
        {
            parsingContext.LogError(reader, "Nenhum comando de storyboard válido encontrado.");
            return null;
        }

        return new BlockNode(parsed.Block, parsed.When, parsed.While);
    }
}
