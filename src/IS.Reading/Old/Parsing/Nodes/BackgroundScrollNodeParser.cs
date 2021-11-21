using IS.Reading.Navigation;
using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public interface IBackgroundScrollNodeParser : INodeParser
{
}

public class BackgroundScrollNodeParser : IBackgroundScrollNodeParser
{
    private readonly IElementParser elementParser;

    public BackgroundScrollNodeParser(IElementParser elementParser, IWhen)
    {
        this.elementParser = elementParser;
    }

    public string ElementName => "scroll";

    public Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        throw new NotImplementedException();
    }

    protected override INode? CreateNode(XmlReader reader, IParsingContext parsingContext, ElementParsedData parsed)
        => new BackgroundScrollNode(parsed.When);
}
