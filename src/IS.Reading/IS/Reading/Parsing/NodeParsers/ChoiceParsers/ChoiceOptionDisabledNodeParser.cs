using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionDisabledNodeParser : IChoiceOptionDisabledNodeParser
{
    public bool IsArgumentRequired => false;

    public string Name => "disabled";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var ctx = (ChoiceOptionParentParsingContext)parentParsingContext;
        ctx.Builders.Add(new ChoiceOptionIsEnabledSetter(false));
        return Task.CompletedTask;
    }
}
