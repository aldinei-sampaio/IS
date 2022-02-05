using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceDefaultNodeParser : IChoiceDefaultNodeParser
{
    private readonly INameTextParser textParser;

    public ChoiceDefaultNodeParser(INameTextParser textParser)
        => this.textParser = textParser;

    public string Name => "default";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (string.IsNullOrWhiteSpace(reader.Argument))
        {
            parsingContext.LogError(reader, "A opção padrão não foi informada.");
            return Task.CompletedTask;
        }

        var parsed = textParser.Parse(reader, parsingContext, reader.Argument);
        if (parsed is null)
            return Task.CompletedTask;

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.Choice.Default = parsed;

        return Task.CompletedTask;
    }
}
