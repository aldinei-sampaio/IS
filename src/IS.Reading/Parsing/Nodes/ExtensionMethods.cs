using IS.Reading.Navigation;
using IS.Reading.Parsing.Attributes;

namespace IS.Reading.Parsing.Nodes;

public static class ExtensionMethods
{
    public static void Add(this ParserDictionary<IAttributeParser> dic, IAttributeParser parser)
        => dic.Add(parser.ElementName, parser);

    public static void Add(this ParserDictionary<INodeParser> dic, INodeParser parser)
        => dic.Add(parser.ElementName, parser);

    public static void Add(this IBlock block, INode node)
        => block.ForwardQueue.Enqueue(node);

    public static bool IsEmpty(this IBlock block)
        =>  block.ForwardQueue.Count == 0;
}
