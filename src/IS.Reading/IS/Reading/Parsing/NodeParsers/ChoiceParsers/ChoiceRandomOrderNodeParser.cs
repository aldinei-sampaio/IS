using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceRandomOrderNodeParser : IChoiceRandomOrderNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public ChoiceRandomOrderNodeParser(IElementParser elementParser)
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal();
    }

    public string Name => "randomorder";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.Choice.RandomOrder = true;
    }
}