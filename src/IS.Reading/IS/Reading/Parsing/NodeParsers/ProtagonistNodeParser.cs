using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class ProtagonistNodeParser : IProtagonistNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public ProtagonistNodeParser(
        IElementParser elementParser,
        IWhenAttributeParser whenAttributeParser,
        INameTextParser nameTextParser
    )
    {
        this.elementParser = elementParser;
        Settings = new ElementParserSettings(whenAttributeParser, nameTextParser);
    }

    public string Name => "protagonist";

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = await elementParser.ParseAsync(reader, parsingContext, Settings);

        if (parsed.Text is null)
            return null;

        if (parsed.Text.Length == 0)
        {
            parsingContext.LogError(reader, "Era esperado o nome do personagem.");
            return null;
        }

        return new ProtagonistChangeNode(parsed.Text, parsed.When);
    }

    public INode? DismissNode { get; } 
        = DismissNode<ProtagonistChangeNode>.Create(new(string.Empty, null));
}
