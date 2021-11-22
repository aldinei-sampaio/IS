namespace IS.Reading.Parsing;

public class ElementParserSettings : IElementParserSettings
{
    public ITextParser? TextParser { get; set; }

    public ParserDictionary<IAttributeParser> AttributeParsers { get; } = new();

    public ParserDictionary<INodeParser> ChildParsers { get; } = new();

    public ElementParserSettings(params object[] parsers)
    {
        foreach(var parser in parsers)
        {
            switch(parser)
            {
                case IAttributeParser attributeParser:
                    AttributeParsers.Add(attributeParser.ElementName, attributeParser);
                    break;
                case INodeParser nodeParser:
                    ChildParsers.Add(nodeParser.ElementName, nodeParser);
                    break;
                case ITextParser textParser:
                    TextParser = textParser;
                    break;
                default:
                    throw new ArgumentException($"Argumento do tipo '{parser.GetType().Name}' não é válido.");
            }
        }
    }
}
