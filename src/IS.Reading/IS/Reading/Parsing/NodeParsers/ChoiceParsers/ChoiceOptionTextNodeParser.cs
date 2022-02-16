using IS.Reading.Choices;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionTextNodeParser : IChoiceOptionTextNodeParser
{
    private readonly ITextSourceParser textSourceParser;

    public ChoiceOptionTextNodeParser(ITextSourceParser textSourceParser)
        => this.textSourceParser = textSourceParser;

    public bool IsArgumentRequired => true;

    public string Name => "text";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var parseResult = textSourceParser.Parse(reader.Argument);
        if (!parseResult.IsOk)
        {
            parsingContext.LogError(reader, parseResult.ErrorMessage);
            return Task.CompletedTask;
        }

        var ctx = (ChoiceOptionParentParsingContext)parentParsingContext;
        ctx.Builders.Add(new ChoiceOptionTextSetter(parseResult.Value));
        return Task.CompletedTask;
    }
}
