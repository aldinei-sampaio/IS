using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionNodeParser : IChoiceOptionNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public ChoiceOptionNodeParser(
        IElementParser elementParser,
        IWhenAttributeParser whenAttributeParser,
        IBalloonTextParser balloonTextParser,
        INameTextParser nameTextParser,
        IConditionParser conditionParser
    )
    {
        this.elementParser = elementParser;

        Settings = ElementParserSettings.NonRepeat(
            whenAttributeParser,
            balloonTextParser,
            new ChoiceOptionTextNodeParser(elementParser, balloonTextParser),
            new ChoiceOptionEnabledWhenNodeParser(elementParser, balloonTextParser, conditionParser),
            new ChoiceOptionDisabledTextNodeParser(elementParser, balloonTextParser),
            new ChoiceOptionIconNodeParser(elementParser, nameTextParser)
        );
    }

    public string Name => string.Empty;

    public string? NameRegex => "^[a-z]$";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new ChoiceOptionParentParsingContext();
        myContext.Option.Key = reader.LocalName;

        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (myContext.ParsedText is not null)
            myContext.Option.Text = myContext.ParsedText;

        if (string.IsNullOrWhiteSpace(myContext.Option.Text))
        {
            parsingContext.LogError(reader, "O texto da opção não foi informado.");
            return;
        }

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.Choice.Options.Add(myContext.Option);
    }
}
