namespace IS.Reading.Parsing;

public abstract class ElementParserSettings
{
    protected ElementParserSettings(IEnumerable<INodeParser> parsers)
    {
        foreach (var parser in parsers)
            ChildParsers.Add(parser);
    }

    public IParserDictionary<INodeParser> ChildParsers { get; } = new ParserDictionary<INodeParser>();

    public class Block : ElementParserSettings, IElementParserSettings
    {
        public Block(params INodeParser[] parsers) : base(parsers)
        {
        }
        public bool ExitOnUnknownNode => false;
        public bool ExitOnElse => false;
        public bool ExitOnEnd => true;
        public bool NoRepeatNode => false;
        public bool ParseCurrent => false;
    }

    public class NoBlock : ElementParserSettings, IElementParserSettings
    {
        public NoBlock(params INodeParser[] parsers) : base(parsers)
        {
        }
        public bool ExitOnUnknownNode => false;
        public bool ExitOnElse => false;
        public bool ExitOnEnd => false;
        public bool NoRepeatNode => false;
        public bool ParseCurrent => false;
    }

    public class IfBlock : ElementParserSettings, IElementParserSettings
    {
        public IfBlock(params INodeParser[] parsers) : base(parsers)
        {
        }
        public bool ExitOnUnknownNode => false;
        public bool ExitOnElse => true;
        public bool ExitOnEnd => true;
        public bool NoRepeatNode => false;
        public bool ParseCurrent => false;
    }

    public class Aggregated : ElementParserSettings, IElementParserSettings
    {
        public Aggregated(params INodeParser[] parsers) : base(parsers)
        {
        }
        public bool ExitOnUnknownNode => true;
        public bool ExitOnElse => false;
        public bool ExitOnEnd => false;
        public bool NoRepeatNode => false;
        public bool ParseCurrent => false;
    }

    public class AggregatedCurrent : ElementParserSettings, IElementParserSettings
    {
        public AggregatedCurrent(IEnumerable<INodeParser> parsers) : base(parsers)
        {
        }
        public bool ExitOnUnknownNode => true;
        public bool ExitOnElse => false;
        public bool ExitOnEnd => false;
        public bool NoRepeatNode => false;
        public bool ParseCurrent => true;
    }

    public class AggregatedNonRepeat : ElementParserSettings, IElementParserSettings
    {
        public AggregatedNonRepeat(params INodeParser[] parsers) : base(parsers)
        {
        }
        public bool ExitOnUnknownNode => true;
        public bool ExitOnElse => false;
        public bool ExitOnEnd => false;
        public bool NoRepeatNode => true;
        public bool ParseCurrent => false;
    }
}
