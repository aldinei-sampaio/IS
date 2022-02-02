using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class MoodNodeParser : IMoodNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public MoodNodeParser(
        IElementParser elementParser, 
        IMoodTextParser moodTextParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(moodTextParser);
    }

    public string Name => "mood";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var parsedText = myContext.ParsedText;
        if (parsedText is null)
            return;

        if (parsingContext.SceneContext.HasMood)
        {
            parsingContext.LogError(reader, "Mais de uma definição de humor para a mesma cena.");
            return;
        }
        parsingContext.SceneContext.HasMood = true;

        var moodType = Enum.Parse<MoodType>(parsedText);
        parentParsingContext.AddNode(new MoodNode(moodType));
    }
}
