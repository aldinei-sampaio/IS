using IS.Reading.Navigation;
using IS.Reading.Nodes;
using System.Xml.Linq;
using ParserDictionary = System.Collections.Generic.Dictionary<string, System.Func<System.Xml.Linq.XElement, IS.Reading.Navigation.INode>>;

namespace IS.Reading.Parsing.NodeParsers;

public struct BackgroundParser : INodeParser
{
    private readonly ParserDictionary parsers = InitializeParsers();

    private static ParserDictionary InitializeParsers()
    {
        return new ParserDictionary(StringComparer.OrdinalIgnoreCase)
            {
                { "color", i => new BackgroundColorNode(i.Value, i.ParseCondition()) },
                { "left", i => new BackgroundLeftNode(i.Value, i.ParseCondition()) },
                { "right", i => new BackgroundRightNode(i.Value, i.ParseCondition()) },
                { "scroll", i => new BackgroundScrollNode(i.ParseCondition()) },
                { "pause", i => new PauseNode(i.ParseCondition()) }
            };
    }

    public INode Parse(XElement element)
    {
        Block block;

        if (element.HasElements)
        {
            block = new();
            foreach (var child in element.Elements())
            {
                if (!parsers.TryGetValue(element.Name.LocalName, out var parser))
                    throw new ParsingException($"Elemento não reconhecido: background.{element.Name.LocalName}");
                block.ForwardQueue.Enqueue(parser.Invoke(child));
            }
        }
        else
        {
            block = new Block(
                new BackgroundLeftNode(element.Value, null),
                new BackgroundScrollNode(null)
            );
        }

        return new BlockNode(block, element.ParseCondition());
    }
}
