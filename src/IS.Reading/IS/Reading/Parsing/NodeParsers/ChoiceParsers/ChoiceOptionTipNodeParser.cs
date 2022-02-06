using IS.Reading.Choices;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionTipNodeParser : IChoiceOptionTipNodeParser
{
    private readonly ITextSourceParser textSourceParser;

    public ChoiceOptionTipNodeParser(ITextSourceParser textSourceParser)
        => this.textSourceParser = textSourceParser;

    public bool IsArgumentRequired => true;

    public string Name => "tip";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (string.IsNullOrEmpty(reader.Argument))
            throw new InvalidOperationException();

        var parseResult = textSourceParser.Parse(reader.Argument);
        if (!parseResult.IsOk)
        {
            parsingContext.LogError(reader, parseResult.ErrorMessage);
            return Task.CompletedTask;
        }

        var ctx = (ChoiceOptionParentParsingContext)parentParsingContext;
        ctx.Builders.Add(new ChoiceOptionTipBuilder(parseResult.Value));
        return Task.CompletedTask;
    }
}
