using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceDefaultNodeParser : IChoiceDefaultNodeParser
{
    public INameArgumentParser NameArgumentParser { get; }

    public ChoiceDefaultNodeParser(INameArgumentParser nameArgumentParser)
        => NameArgumentParser = nameArgumentParser;

    public bool IsArgumentRequired => true;

    public string Name => "default";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = NameArgumentParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return Task.CompletedTask;
        }

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.Builders.Add(new ChoiceDefaultSetter(result.Value));

        return Task.CompletedTask;
    }
}
