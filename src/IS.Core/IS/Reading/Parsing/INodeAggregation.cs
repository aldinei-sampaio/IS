namespace IS.Reading.Parsing;

public interface INodeAggregation
{
    IParserDictionary<INodeParser> ChildParsers { get; }
}
