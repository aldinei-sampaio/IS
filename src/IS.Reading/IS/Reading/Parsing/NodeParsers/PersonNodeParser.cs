using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.NodeParsers.PersonParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class PersonNodeParser : IPersonNodeParser
{
    private readonly IElementParser elementParser;
    private readonly INodeParser childParser;

    public IElementParserSettings Settings { get; }

    public IElementParserSettings AggregationSettings { get; }

    public PersonNodeParser(
        IElementParser elementParser, 
        IPersonTextNodeParser personTextNodeParser,
        ISpeechNodeParser speechNodeParser,
        IThoughtNodeParser thoughtNodeParser,
        IMoodNodeParser moodNodeParser,
        IPauseNodeParser pauseNodeParser,
        ISetNodeParser setNodeParser,
        IUnsetNodeParser unsetNodeParser
    )
    {
        this.elementParser = elementParser;
        this.childParser = personTextNodeParser;
        Settings = ElementParserSettings.AggregatedNonRepeat(childParser);

        AggregationSettings = ElementParserSettings.Aggregated(
            speechNodeParser, 
            thoughtNodeParser,
            moodNodeParser,
            pauseNodeParser,
            setNodeParser,
            unsetNodeParser
        );
    }

    public string Name => childParser.Name;

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new BlockParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var parsedText = myContext.ParsedText;

        if (parsedText is null)
            return;

        if (parsedText.Length == 0)
        {
            parsingContext.LogError(reader, "Era esperado o nome do personagem.");
            return;
        }

        if (reader.ReadState == ReadState.EndOfFile)
            return;

        await elementParser.ParseAsync(reader, parsingContext, myContext, AggregationSettings);

        if (myContext.Nodes.Count == 0)
            return;

        if (parsingContext.SceneContext.HasMood)
        {
            parsingContext.LogError(reader, "Foi definido humor mas não foi definida uma fala ou pensamento correspondente.");
            return;
        }

        myContext.Nodes.Insert(0, InitializeMoodNode);
        myContext.Nodes.Add(DismissMoodNode);

        var node = new PersonNode(parsedText, new Block(myContext.Nodes));
        parentParsingContext.AddNode(node);
    }

    public MoodNode InitializeMoodNode { get; } = new MoodNode(MoodType.Normal);
    public MoodNode DismissMoodNode { get; } = new MoodNode(null);
}