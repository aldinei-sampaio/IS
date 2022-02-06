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
            parsingContext.LogError(reader, "O comando 'randomorder' não suporta argumentos.");
            return Task.CompletedTask;
        }

        var ctx = (ChoiceParentParsingContext)parentParsingContext;
        ctx.AddSetter(i => i.RandomOrder = true);
        return Task.CompletedTask;
    }
}