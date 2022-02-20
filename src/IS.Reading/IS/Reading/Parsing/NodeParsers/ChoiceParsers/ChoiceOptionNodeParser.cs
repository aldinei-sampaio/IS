using IS.Reading.Choices;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionNodeParser : IChoiceOptionNodeParser
{
    public IElementParser ElementParser { get; }
    public ITextSourceParser TextSourceParser { get; }
    public IElementParserSettings Settings { get; }

    public ChoiceOptionNodeParser(
        IElementParser elementParser,
        ITextSourceParser textSourceParser,
        IChoiceOptionTextNodeParser choiceOptionTextNodeParser,
        IChoiceOptionDisabledNodeParser choiceOptionDisabledNodeParser,
        IChoiceOptionIconNodeParser choiceOptionIconNodeParser,
        IChoiceOptionTipNodeParser choiceOptionTipNodeParser,
        IChoiceOptionIfNodeParser choiceOptionIfNodeParser
    )
    {
        this.ElementParser = elementParser;
        this.TextSourceParser = textSourceParser;
        Settings = new ElementParserSettings.Block(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            choiceOptionIfNodeParser
        );
    }

    public bool IsArgumentRequired => false;

    public string Name => string.Empty;

    public string? NameRegex => /* lang=regex */ @"^[A-Za-z0-9_]+\)$";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var key = reader.Command[0..^1].ToLower();
        var myContext = new ChoiceOptionParentParsingContext();
        
        if (string.IsNullOrEmpty(reader.Argument))
        {
            await ElementParser.ParseAsync(reader, parsingContext, myContext, Settings);
            if (!parsingContext.IsSuccess || myContext.Builders.Count == 0)
                return;
        }
        else
        {
            var textSourceParsingResult = TextSourceParser.Parse(reader.Argument);
            if (!textSourceParsingResult.IsOk)
            {
                parsingContext.LogError(reader, textSourceParsingResult.ErrorMessage);
                return;
            }

            myContext.Builders.Add(new ChoiceOptionTextSetter(textSourceParsingResult.Value));
        }

        var builder = new ChoiceOptionBuilder(key, myContext.Builders);

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.Builders.Add(builder);
    }
}
