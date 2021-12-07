namespace IS.Reading.Parsing;

public class ElementParserSettings : IElementParserSettings
{
    private ElementParserSettings()
    {
    }

    public ITextParser? TextParser { get; private set; }

    public IParserDictionary<IAttributeParser> AttributeParsers { get; } = new ParserDictionary<IAttributeParser>();

    public IParserDictionary<INodeParser> ChildParsers { get; } = new ParserDictionary<INodeParser>();

    public bool ExitOnUnknownNode { get; private set; }

    public bool NoRepeatNode { get; private set; }

    public static ElementParserSettings Normal(params object[] parsers)
    { 
        var settings = new ElementParserSettings();

        foreach (var parser in parsers)
        {
            switch(parser)
            {
                case IAttributeParser attributeParser:
                    settings.AttributeParsers.Add(attributeParser);
                    break;
                case INodeParser nodeParser:
                    settings.ChildParsers.Add(nodeParser);
                    break;
                case ITextParser textParser:
                    settings.TextParser = textParser;
                    break;
                default:
                    throw new ArgumentException($"Argumento do tipo '{parser.GetType().Name}' não é válido.");
            }
        }

        return settings;
    }

    public static ElementParserSettings NoRepeat(params object[] parsers)
    {
        var settings = Aggregated(parsers);
        settings.NoRepeatNode = true;
        return settings;
    }

    public static ElementParserSettings Aggregated(params object[] parsers)
    {
        var settings = Normal(parsers);
        settings.ExitOnUnknownNode = true;
        return settings;
    }
}
