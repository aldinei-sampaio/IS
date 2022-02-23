using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceRandomOrderNodeParser : IChoiceRandomOrderNodeParser
{
    public bool IsArgumentRequired => false;

    public string Name => "randomorder";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (!string.IsNullOrEmpty(reader.Argument))
        {
            parsingContext.LogError(reader, "O comando 'randomorder' não suporta argumento.");
            return Task.CompletedTask;
        }

        var ctx = (BuilderParentParsingContext<IChoicePrototype>)parentParsingContext;
        ctx.Builders.Add(new ChoiceRandomOrderSetter(true));
        return Task.CompletedTask;
    }
}