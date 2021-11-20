using IS.Reading.Navigation;
using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class RootNodeParser : NodeParserBase
{
    public static RootNodeParser Instance = new();

    protected RootNodeParser()
    {
        ChildParsers.Add(BackgroundNodeParser.Instance);
        ChildParsers.Add(PauseNodeParser.Instance);
        ChildParsers.Add(DoNodeParser.Instance);
    }

    public override string ElementName => "storyboard";

    protected override INode? CreateNode(XmlReader reader, IParsingContext parsingContext, ParsedData parsed)
    {
        if (parsed.Block is null || parsed.Block.IsEmpty())
        {
            parsingContext.LogError(reader, "Nenhum comando de storyboard válido encontrado.");
            return null;
        }

        return new BlockNode(parsed.Block, null, null);
    }
}
