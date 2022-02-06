using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceDefaultNodeParser : IChoiceDefaultNodeParser
{
    private readonly INameTextParser textParser;

    public ChoiceDefaultNodeParser(INameTextParser textParser)
        => this.textParser = textParser;

    public bool IsArgumentRequired => true;

    public string Name => "default";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = textParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return Task.CompletedTask;
        }

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.AddSetter(i => i.Default = result.Value);

        return Task.CompletedTask;
    }
}
