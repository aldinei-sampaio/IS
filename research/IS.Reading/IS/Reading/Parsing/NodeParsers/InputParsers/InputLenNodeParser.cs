using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.InputParsers;

public class InputLenNodeParser : IInputLenNodeParser
{
    public const int MinValue = 1;
    public const int MaxValue = 32;

    public IIntegerArgumentParser IntegerArgumentParser { get; }

    public InputLenNodeParser(IIntegerArgumentParser integerArgumentParser)
        => IntegerArgumentParser = integerArgumentParser;

    public bool IsArgumentRequired => true;

    public string Name => "len";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var parseResult = IntegerArgumentParser.Parse(reader.Argument, MinValue, MaxValue);
        if (!parseResult.IsOk)
        {
            parsingContext.LogError(reader, parseResult.ErrorMessage);
            return Task.CompletedTask;
        }

        var ctx = (InputParentParsingContext)parentParsingContext;
        ctx.InputBuilder.MaxLength = parseResult.Value;
        return Task.CompletedTask;
    }
}