using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.Text;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class BackgroundRightNodeParser : IBackgroundRightNodeParser
{
    private readonly IElementParser elementParser;

    public BackgroundRightNodeParser(
        IElementParser elementParser,
        IBackgroundImageTextParser backgroundImageTextParser
    )
    {
        this.elementParser = elementParser;
        this.elementParser.TextParser = backgroundImageTextParser;
    }

    public string ElementName => "right";

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

        return new BackgroundRightNode(parsed.Text, parsed.When);
    }
}
