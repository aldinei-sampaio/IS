using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionIconNodeParser : IChoiceOptionIconNodeParser
{
    private readonly INameTextParser nameTextParser;

    public ChoiceOptionIconNodeParser(INameTextParser nameTextParser)
        => this.nameTextParser = nameTextParser;

    public bool IsArgumentRequired => true;

    public string Name => "icon";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (string.IsNullOrEmpty(reader.Argument))
            throw new InvalidOperationException();

        var result = nameTextParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return Task.CompletedTask;
        }

        var ctx = (ChoiceOptionParentParsingContext)parentParsingContext;
        ctx.AddSetter(i => i.ImageName = result.Value);
        return Task.CompletedTask;
    }
}
