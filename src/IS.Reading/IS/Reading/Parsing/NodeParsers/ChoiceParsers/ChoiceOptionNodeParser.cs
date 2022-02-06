using IS.Reading.Choices;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionNodeParser : IChoiceOptionNodeParser
{
    private readonly IElementParser elementParser;
    private readonly ITextSourceParser textSourceParser;

    public IElementParserSettings Settings { get; }

    public ChoiceOptionNodeParser(
        IElementParser elementParser,
        ITextSourceParser textSourceParser,
        IChoiceOptionTextNodeParser choiceOptionTextNodeParser,
        IChoiceOptionDisabledNodeParser choiceOptionDisabledNodeParser,
        IChoiceOptionIconNodeParser choiceOptionIconNodeParser,
        IChoiceOptionTipNodeParser choiceOptionHelpTextNodeParser,
        IChoiceOptionIfNodeParser choiceOptionIfNodeParser
    )
    {
        this.elementParser = elementParser;
        this.textSourceParser = textSourceParser;
        Settings = ElementParserSettings.Block(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionHelpTextNodeParser,
            choiceOptionIfNodeParser
        );
    }

    public bool IsArgumentRequired => false;

    public string Name => string.Empty;

    public string? NameRegex => "^[a-z]$";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var key = reader.ElementName;
        var myContext = new ChoiceOptionParentParsingContext();
        
        if (string.IsNullOrEmpty(reader.Argument))
        {
            await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);
            if (!parsingContext.IsSuccess)
                return;
        }
        else
        {
            var textSourceParsingResult = textSourceParser.Parse(reader.Argument);
            if (!textSourceParsingResult.IsOk)
            {
                parsingContext.LogError(reader, textSourceParsingResult.ErrorMessage);
                return;
            }

            myContext.Builders.Add(new ChoiceOptionTextBuilder(textSourceParsingResult.Value));
        }

        var builder = new ChoiceOptionBuilder(key, myContext.Builders);

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.Builders.Add(builder);
    }
}
