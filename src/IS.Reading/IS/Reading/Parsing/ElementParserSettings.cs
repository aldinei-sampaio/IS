namespace IS.Reading.Parsing;

public class ElementParserSettings : IElementParserSettings
{
    private ElementParserSettings()
    {
    }

    public IParserDictionary<INodeParser> ChildParsers { get; } = new ParserDictionary<INodeParser>();

    public bool ExitOnUnknownNode { get; private set; }

    public bool ExitOnElse { get; private set; }

    public bool IsBlock { get; private set; }

    public bool NoRepeatNode { get; private set; }

    public static ElementParserSettings IfBlock(params INodeParser[] parsers)
    {
        var settings = Block(parsers);
        settings.ExitOnElse = true;
        settings.IsBlock = true;
        return settings;
    }

    public static ElementParserSettings Block(params INodeParser[] parsers)
    {
        var settings = NoBlock(parsers);
        settings.IsBlock = true;
        return settings;
    }

    public static ElementParserSettings NoBlock(params INodeParser[] parsers)
    { 
        var settings = new ElementParserSettings();

        foreach (var parser in parsers)
            settings.ChildParsers.Add(parser);

        return settings;
    }

    public static ElementParserSettings Aggregated(params INodeParser[] parsers)
    {
        var settings = NoBlock(parsers);
        settings.ExitOnUnknownNode = true;
        return settings;
    }

    public static ElementParserSettings AggregatedNonRepeat(params INodeParser[] parsers)
    {
        var settings = Aggregated(parsers);
        settings.NoRepeatNode = true;
        return settings;
    }
}
