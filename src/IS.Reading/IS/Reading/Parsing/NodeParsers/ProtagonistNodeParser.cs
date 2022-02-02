using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.ArgumentParsers;
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
        Settings = ElementParserSettings.Normal(whenAttributeParser, nameTextParser);
    }

    public string Name => "protagonist";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var parsedText = myContext.ParsedText;

        if (parsedText is null)
            return;

        var node = new ProtagonistNode(parsedText.Length == 0 ? null : parsedText, myContext.When);
        parentParsingContext.AddNode(node);
        parsingContext.RegisterDismissNode(DismissNode);
    }

    public INode DismissNode { get; } 
        = new ProtagonistNode(null, null);
}
