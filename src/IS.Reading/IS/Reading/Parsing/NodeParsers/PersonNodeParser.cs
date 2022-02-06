using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Parsing.NodeParsers.PersonParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class PersonNodeParser : IPersonNodeParser
{
    private readonly IElementParser elementParser;
    private readonly INameTextParser nameTextParser;

    public IElementParserSettings Settings { get; }

    public PersonNodeParser(
        IElementParser elementParser, 
        INameTextParser nameTextParser,
        ISpeechNodeParser speechNodeParser,
        IThoughtNodeParser thoughtNodeParser,
        IMoodNodeParser moodNodeParser,
        IPauseNodeParser pauseNodeParser,
        ISetNodeParser setNodeParser
    )
    {
        this.elementParser = elementParser;
        this.nameTextParser = nameTextParser;
        Settings = ElementParserSettings.Aggregated(
            speechNodeParser, 
            thoughtNodeParser,
            moodNodeParser,
            pauseNodeParser,
            setNodeParser
        );
    }

    public bool IsArgumentRequired => true;

    public string Name => "@";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var parsedName = nameTextParser.Parse(reader.Argument);
        if (!parsedName.IsOk)
        {
            parsingContext.LogError(reader, parsedName.ErrorMessage);
            return;
        }

        var myContext = new ParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (myContext.Nodes.Count == 0)
            return;

        if (parsingContext.SceneContext.HasMood)
        {
            parsingContext.LogError(reader, "Foi definido humor mas não foi definida uma fala ou pensamento correspondente.");
            return;
        }

        myContext.Nodes.Insert(0, InitializeMoodNode);
        myContext.Nodes.Add(DismissMoodNode);

        var block = parsingContext.BlockFactory.Create(myContext.Nodes);
        var node = new PersonNode(parsedName.Value, block);
        parentParsingContext.AddNode(node);
    }

    public MoodNode InitializeMoodNode { get; } = new MoodNode(MoodType.Normal);
    public MoodNode DismissMoodNode { get; } = new MoodNode(null);
}