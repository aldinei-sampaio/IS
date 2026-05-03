using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Parsing.NodeParsers.PersonParsers;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class PersonNodeParser : IPersonNodeParser
{
    public IElementParser ElementParser { get; }
    public INameArgumentParser NameArgumentParser { get; }
    public IElementParserSettings Settings { get; }

    public PersonNodeParser(
        IElementParser elementParser, 
        INameArgumentParser nameArgumentParser,
        ISpeechNodeParser speechNodeParser,
        IThoughtNodeParser thoughtNodeParser,
        IMoodNodeParser moodNodeParser,
        IPauseNodeParser pauseNodeParser,
        ISetNodeParser setNodeParser,
        ITitleNodeParser titleNodeParser
    )
    {
        this.ElementParser = elementParser;
        this.NameArgumentParser = nameArgumentParser;
        Settings = new ElementParserSettings.Aggregated(
            speechNodeParser, 
            thoughtNodeParser,
            moodNodeParser,
            pauseNodeParser,
            setNodeParser,
            titleNodeParser
        );
    }

    public bool IsArgumentRequired => true;

    public string Name => "@";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var parsedName = NameArgumentParser.Parse(reader.Argument);
        if (!parsedName.IsOk)
        {
            parsingContext.LogError(reader, parsedName.ErrorMessage);
            return;
        }

        var myContext = new ParentParsingContext();
        await ElementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (!parsingContext.IsSuccess || myContext.Nodes.Count == 0)
            return;

        if (!HasCustomNode<BalloonTitleNode>(myContext.Nodes))
        {
            var title = new VariableTextSource(parsedName.Value);
            myContext.Nodes.Insert(0, new BalloonTitleNode(title));
        }

        if (!HasCustomNode<MoodNode>(myContext.Nodes))
            myContext.Nodes.Insert(0, InitializeMoodNode);

        myContext.Nodes.Add(DismissTitleNode);
        myContext.Nodes.Add(DismissMoodNode);

        var block = parsingContext.BlockFactory.Create(myContext.Nodes);
        var node = new PersonNode(parsedName.Value, block);
        parentParsingContext.AddNode(node);
    }

    private static bool HasCustomNode<T>(IEnumerable<INode> nodes) where T : INode
    {
        foreach (var node in nodes)
        {
            if (node is IPauseNode || node.ChildBlock is not null)
                return false;

            if (node is T)
                return true;
        }

        return false;
    }

    public MoodNode InitializeMoodNode { get; } = new MoodNode(MoodType.Normal);
    public MoodNode DismissMoodNode { get; } = new MoodNode(null);
    public BalloonTitleNode DismissTitleNode { get; } = new BalloonTitleNode(null);
}