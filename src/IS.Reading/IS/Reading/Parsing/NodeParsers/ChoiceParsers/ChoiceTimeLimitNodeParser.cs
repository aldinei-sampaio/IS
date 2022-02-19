using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceTimeLimitNodeParser : IChoiceTimeLimitNodeParser
{
    public const int MinValue = 1;
    public const int MaxValue = 30000;

    public IIntegerArgumentParser IntegerArgumentParser { get; }

    public ChoiceTimeLimitNodeParser(IIntegerArgumentParser integerParser)
        => IntegerArgumentParser = integerParser;

    public bool IsArgumentRequired => true;

    public string Name => "timelimit";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var valueParsingResult = IntegerArgumentParser.Parse(reader.Argument, MinValue, MaxValue);
        if (!valueParsingResult.IsOk)
        {
            parsingContext.LogError(reader, valueParsingResult.ErrorMessage);
            return Task.CompletedTask;
        }

        var timeLimit = TimeSpan.FromMilliseconds(valueParsingResult.Value);

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.Builders.Add(new ChoiceTimeLimitSetter(timeLimit));
        
        return Task.CompletedTask;
    }
}
