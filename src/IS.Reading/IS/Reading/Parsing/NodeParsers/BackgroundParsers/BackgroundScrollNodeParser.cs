using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundScrollNodeParser : IBackgroundScrollNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public BackgroundScrollNodeParser(IElementParser elementParser, IWhenAttributeParser whenAttributeParser)
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(whenAttributeParser);
    }

    public string Name => "scroll";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
        var node = new ScrollNode(myContext.When);
        parentParsingContext.AddNode(node);
    }
}
