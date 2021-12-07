using IS.Reading.Events;
using IS.Reading.Navigation;
using Microsoft.Extensions.DependencyInjection;
using System.Xml;

namespace IS.Reading.Parsing;

public class StoryboardParser : IStoryboardParser
{
    private readonly IServiceProvider serviceProvider;

    public StoryboardParser(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task<IStoryboard> ParseAsync(TextReader textReader)
    {
        var context = serviceProvider.GetRequiredService<IParsingContext>();

        using var reader = XmlReader.Create(textReader, new() { Async = true, CloseInput = true });

        var rootBlockParser = serviceProvider.GetRequiredService<IRootBlockParser>();

        var parsed = await rootBlockParser.ParseAsync(reader, context);

        if (!context.IsSuccess)
            throw new ParsingException(context.ToString()!);

        foreach(var node in context.DismissNodes.Reverse())
            parsed.ForwardQueue.Enqueue(node);

        var sceneNavigator = serviceProvider.GetRequiredService<ISceneNavigator>();
        var eventManager = serviceProvider.GetRequiredService<IEventManager>();

        return new Storyboard(parsed, sceneNavigator, eventManager);
    }
}
