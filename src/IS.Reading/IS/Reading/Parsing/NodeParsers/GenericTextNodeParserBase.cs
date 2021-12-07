using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public abstract class GenericTextNodeParserBase : INodeParser
{
    private readonly IElementParser elementParser;
    
    public IElementParserSettings Settings { get; }

    public GenericTextNodeParserBase(IElementParser elementParser, ITextParser textParser)
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(textParser);
    }

    public abstract string Name { get; }

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
        => await elementParser.ParseAsync(reader, parsingContext, parentParsingContext, Settings);
}
