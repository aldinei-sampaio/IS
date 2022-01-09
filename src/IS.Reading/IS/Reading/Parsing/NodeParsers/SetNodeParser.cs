using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.Variables;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class SetNodeParser : ISetNodeParser
{
    private readonly IVarSetParser varSetParser;
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public SetNodeParser(
        IVarSetParser varSetParser,
        IElementParser elementParser,
        IVarSetTextParser varSetTextParser
    )
    {
        this.varSetParser = varSetParser;
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(varSetTextParser);
    }

    public string Name => "set";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var parsedText = myContext.ParsedText;

        if (parsedText is null)
            return;

        var varSet = varSetParser.Parse(parsedText);

        if (varSet is null)
            parsingContext.LogError(reader, "Expressão de atribuição de variável inválida.");
        else
            parentParsingContext.AddNode(new VarSetNode(varSet));
    }
}
