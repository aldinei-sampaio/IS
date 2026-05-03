using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionDisabledNodeParser : IChoiceOptionDisabledNodeParser
{
    public bool IsArgumentRequired => false;

    public string Name => "disabled";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (!string.IsNullOrEmpty(reader.Argument))
        {
            parsingContext.LogError(reader, "O comando 'disabled' não suporta argumento.");
            return Task.CompletedTask;
        }
        var ctx = (BuilderParentParsingContext<IChoiceOptionPrototype>)parentParsingContext;
        ctx.Builders.Add(new ChoiceOptionIsEnabledSetter(false));
        return Task.CompletedTask;
    }
}
