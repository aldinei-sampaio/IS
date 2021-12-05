using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceTimeLimitNodeParser : IChoiceTimeLimitNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public ChoiceTimeLimitNodeParser(IElementParser elementParser, ITextParser textParser)
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(textParser);
    }

    public string Name => "timelimit";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
        if (myContext.ParsedText is null)
            return;

        var value = int.Parse(myContext.ParsedText);
        if (value < 1)
            return;

        if (value > 30000)
        {
            parsingContext.LogError(reader, "O limite de tempo não pode ser maior que 30 segundos.");
            return;
        }

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.Choice.TimeLimit = TimeSpan.FromMilliseconds(value);
    }
}
