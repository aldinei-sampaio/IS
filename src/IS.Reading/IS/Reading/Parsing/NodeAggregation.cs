namespace IS.Reading.Parsing;

public class NodeAggregation : INodeAggregation
{
    public IParserDictionary<INodeParser> ChildParsers { get; }

    public NodeAggregation(params INodeParser[] nodeParsers)
    {
        var parsers = new ParserDictionary<INodeParser>();

        foreach (var nodeParser in nodeParsers)
            parsers.Add(nodeParser);

        ChildParsers = parsers;
    }
}
