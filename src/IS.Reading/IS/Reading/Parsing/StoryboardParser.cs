using IS.Reading.Events;
using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing;

public class StoryboardParser : IStoryboardParser
{
    private readonly IRootBlockParser rootBlockParser;
    private readonly ISceneNavigator sceneNavigator;

    public StoryboardParser(IRootBlockParser rootBlockParser, ISceneNavigator sceneNavigator)
    {
        this.rootBlockParser = rootBlockParser;
        this.sceneNavigator = sceneNavigator;
    }

    public async Task<IStoryboard> ParseAsync(TextReader textReader)
    {
        var context = new ParsingContext();
        using var reader = XmlReader.Create(textReader, new() { Async = true });
        reader.MoveToContent();
        var parsed = await rootBlockParser.ParseAsync(reader, context);
        if (!context.IsSuccess)
            throw new ParsingException(context.ToString());
        return new Storyboard(parsed!, sceneNavigator, new EventManager());
    }
}
