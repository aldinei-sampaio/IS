using IS.Reading.Nodes;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class MoodNodeParser : IMoodNodeParser
{
    public bool IsArgumentRequired => true;

    public string Name => "#";

    public Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        if (!Enum.TryParse<MoodType>(reader.Argument, out var moodType))
        {
            parsingContext.LogError(reader, $"O valor '{reader.Argument}' não representa uma emoção válida.");
            return Task.CompletedTask;
        }

        if (parsingContext.SceneContext.HasMood)
        {
            parsingContext.LogError(reader, "Mais de uma definição de humor para a mesma cena.");
            return Task.CompletedTask;
        }

        parsingContext.SceneContext.HasMood = true;

        parentParsingContext.AddNode(new MoodNode(moodType));
        return Task.CompletedTask;
    }
}
