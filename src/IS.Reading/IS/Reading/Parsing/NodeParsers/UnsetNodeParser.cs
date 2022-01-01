using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.Variables;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class UnsetNodeParser : IUnsetNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public UnsetNodeParser(
        IElementParser elementParser,
        INameTextParser nameTextParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(nameTextParser);
    }

    public string Name => "unset";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var parsedText = myContext.ParsedText;

        if (parsedText is null)
            return;

        var varSet = new VarUnset(parsedText);

        parentParsingContext.AddNode(new VarSetNode(varSet));
    }
}
