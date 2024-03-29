﻿using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;
using IS.Reading.Variables;
using Microsoft.Extensions.DependencyInjection;

namespace IS.Reading.Parsing;

public class StoryboardParser : IStoryboardParser
{
    private readonly IServiceProvider serviceProvider;

    public StoryboardParser(IServiceProvider serviceProvider)
        => this.serviceProvider = serviceProvider;

    public async Task<IStoryboard> ParseAsync(IDocumentReader reader)
    {
        using (reader)
        {
            var context = serviceProvider.GetRequiredService<IParsingContext>();
            var rootBlockParser = serviceProvider.GetRequiredService<IRootBlockParser>();

            var parsed = await rootBlockParser.ParseAsync(reader, context);

            if (!context.IsSuccess)
                throw new ParsingException(context.ToString()!);

            foreach (var node in context.DismissNodes.Reverse())
                parsed.Add(node);

            var rootBlock = context.BlockFactory.Create(parsed);
            var rootBlockState = serviceProvider.GetRequiredService<IBlockState>();
            var sceneNavigator = serviceProvider.GetRequiredService<ISceneNavigator>();
            var eventManager = serviceProvider.GetRequiredService<IEventManager>();
            var randomizer = serviceProvider.GetRequiredService<IRandomizer>();
            var navigationState = serviceProvider.GetRequiredService<INavigationState>();
            var variableDictionary = serviceProvider.GetRequiredService<IVariableDictionary>();

            var navigationContext = new NavigationContext(
                rootBlock,
                rootBlockState,
                eventManager,
                randomizer,
                navigationState,
                variableDictionary
            );

            return new Storyboard(navigationContext, sceneNavigator, eventManager);
        }
    }
}
