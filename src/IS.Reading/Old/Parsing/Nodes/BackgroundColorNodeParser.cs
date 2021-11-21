using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.Text;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public class BackgroundColorNodeParser : IBackgroundColorNodeParser
{
    private readonly IElementParser elementParser;

    public BackgroundColorNodeParser(
        IElementParser elementParser, 
        IColorTextParser colorTextParser
    )
    {
        this.elementParser = elementParser;
        this.elementParser.TextParser = colorTextParser;
    }

    public string ElementName => "color";

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext);

        if (parsed is null)
            return null;

        if (string.IsNullOrEmpty(parsed.Text))
        {
            parsingContext.LogError(reader, "Era esperado o nome da cor.");
            return null;
        }

        return new BackgroundColorNode(parsed.Text, parsed.When);
    }
}
