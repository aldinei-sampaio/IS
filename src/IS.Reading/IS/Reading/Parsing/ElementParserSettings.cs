namespace IS.Reading.Parsing;

public class ElementParserSettings : IElementParserSettings
{
    public ITextParser? TextParser { get; set; }

    public IParserDictionary<IAttributeParser> AttributeParsers { get; }         

    public IParserDictionary<INodeParser> ChildParsers { get; }

    public ElementParserSettings(params object[] parsers)
    {
        AttributeParsers = new ParserDictionary<IAttributeParser>();
        ChildParsers = new ParserDictionary<INodeParser>();

        foreach (var parser in parsers)
        {
            switch(parser)
            {
                case IAttributeParser attributeParser:
                    AttributeParsers.Add(attributeParser);
                    break;
                case INodeParser nodeParser:
                    ChildParsers.Add(nodeParser);
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
