using IS.Reading.Navigation;
using IS.Reading.Nodes;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class RootNodeParser : IRootNodeParser
{
    private readonly IElementParser elementParser;

    public RootNodeParser(
        IElementParser elementParser, 
        IBackgroundNodeParser backgroundNodeParser,
        IPauseNodeParser pauseNodeParser,
        IDoNodeParser doNodeParser
    )
    {
        this.elementParser = elementParser;
        elementParser.ChildParsers.Add(backgroundNodeParser);
        elementParser.ChildParsers.Add(pauseNodeParser);
        elementParser.ChildParsers.Add(doNodeParser);
    }

    public string ElementName => "storyboard";

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext);
        
        if (parsed is null)
            return null;

        if (parsed.Block is null || parsed.Block.IsEmpty())
        {
            parsingContext.LogError(reader, "Nenhum comando de storyboard válido encontrado.");
            return null;
        }

        return new BlockNode(parsed.Block, null, null);
    }
}
