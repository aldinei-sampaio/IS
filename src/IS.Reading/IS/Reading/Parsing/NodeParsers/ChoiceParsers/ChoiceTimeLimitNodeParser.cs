using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceTimeLimitNodeParser : IChoiceTimeLimitNodeParser
{
    private readonly IIntegerArgumentParser integerParser;

    public ChoiceTimeLimitNodeParser(IIntegerArgumentParser integerParser)
        => this.integerParser = integerParser;

    public bool IsArgumentRequired => true;

    public string Name => "timelimit";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (string.IsNullOrEmpty(reader.Argument))
            throw new InvalidOperationException();

        var valueParsingResult = integerParser.Parse(reader.Argument, 1, 30000);
        if (!valueParsingResult.IsOk)
        {
            parsingContext.LogError(reader, valueParsingResult.ErrorMessage);
            return Task.CompletedTask;
        }

        var timeLimit = TimeSpan.FromSeconds(valueParsingResult.Value);

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.Builders.Add(new ChoiceTimeLimitSetter(timeLimit));
        
        return Task.CompletedTask;
    }
}
