using IS.Reading.Choices;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionTipNodeParser : IChoiceOptionTipNodeParser
{
    public ITextSourceParser TextSourceParser { get; }

    public ChoiceOptionTipNodeParser(ITextSourceParser textSourceParser)
        => TextSourceParser = textSourceParser;

    public bool IsArgumentRequired => true;

    public string Name => "tip";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (string.IsNullOrEmpty(reader.Argument))
            throw new InvalidOperationException();

        var parseResult = TextSourceParser.Parse(reader.Argument);
        if (!parseResult.IsOk)
        {
            parsingContext.LogError(reader, parseResult.ErrorMessage);
            return Task.CompletedTask;
        }

        var ctx = (ChoiceOptionParentParsingContext)parentParsingContext;
        ctx.Builders.Add(new ChoiceOptionTipSetter(parseResult.Value));
        return Task.CompletedTask;
    }
}
