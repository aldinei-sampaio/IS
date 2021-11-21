using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.Text;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class BackgroundLeftNodeParser : IBackgroundLeftNodeParser
{
    private readonly IElementParser elementParser;

    public BackgroundLeftNodeParser(
        IElementParser elementParser, 
        IBackgroundImageTextParser backgroundImageTextParser
    )
    {
        this.elementParser = elementParser;
        this.elementParser.TextParser = backgroundImageTextParser;
    }

    public string ElementName => "left";

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext);

        if (parsed is null)
            return null;

        if (string.IsNullOrEmpty(parsed.Text))
        {
            parsingContext.LogError(reader, "Era esperado o nome da imagem.");
            return null;
        }

        return new BackgroundLeftNode(parsed.Text, parsed.When);
    }
}
