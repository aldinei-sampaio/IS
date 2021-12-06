﻿using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.Parsing;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.NodeParsers;
using IS.Reading.Parsing.NodeParsers.BackgroundParsers;
using IS.Reading.Parsing.NodeParsers.BalloonParsers;
using IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;
using IS.Reading.Parsing.NodeParsers.ChoiceParsers;
using IS.Reading.Parsing.NodeParsers.PersonParsers;
using IS.Reading.Parsing.TextParsers;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace IS.Reading;

[ExcludeFromCodeCoverage]
public static class ServiceProviderExtensionMethods
{
    public static IServiceCollection AddISReading(this IServiceCollection services)
    {
        // Auxiliar parsers
        services.AddSingleton<IConditionParser, ConditionParser>();
        services.AddSingleton<IElementParser, ElementParser>();

        // Attribute parsers
        services.AddSingleton<IWhenAttributeParser, WhenAttributeParser>();
        services.AddSingleton<IWhileAttributeParser, WhileAttributeParser>();

        // Text parsers
        services.AddSingleton<IColorTextParser, ColorTextParser>();
        services.AddSingleton<IBackgroundImageTextParser, BackgroundImageTextParser>();
        services.AddSingleton<IIntegerTextParser, IntegerTextParser>();
        services.AddSingleton<INameTextParser, NameTextParser>();
        services.AddSingleton<IMoodTextParser, MoodTextParser>();
        services.AddSingleton<IBalloonTextParser, BalloonTextParser>();

        // Node parsers
        services.AddSingleton<IMusicNodeParser, MusicNodeParser>();
        services.AddSingleton<IProtagonistNodeParser, ProtagonistNodeParser>();
        services.AddSingleton<IPauseNodeParser, PauseNodeParser>();
        services.AddSingleton<IBackgroundNodeParser, BackgroundNodeParser>();
        services.AddSingleton<IBlockNodeParser, BlockNodeParser>();
        services.AddSingleton<IMoodNodeParser, MoodNodeParser>();
        services.AddSingleton<IPersonNodeParser, PersonNodeParser>();
        services.AddSingleton<IChoiceNodeParser, ChoiceNodeParser>();

        services.AddSingleton<IBackgroundLeftNodeParser, BackgroundLeftNodeParser>();
        services.AddSingleton<IBackgroundRightNodeParser, BackgroundRightNodeParser>();
        services.AddSingleton<IBackgroundColorNodeParser, BackgroundColorNodeParser>();
        services.AddSingleton<IBackgroundScrollNodeParser, BackgroundScrollNodeParser>();

        services.AddSingleton<ISpeechNodeParser, SpeechNodeParser>();
        services.AddSingleton<IThoughtNodeParser, ThoughtNodeParser>();
        services.AddSingleton<INarrationNodeParser, NarrationNodeParser>();
        services.AddSingleton<ITutorialNodeParser, TutorialNodeParser>();
        services.AddSingleton<ISpeechChildNodeParser, SpeechChildNodeParser>();
        services.AddSingleton<IThoughtChildNodeParser, ThoughtChildNodeParser>();
        services.AddSingleton<INarrationChildNodeParser, NarrationChildNodeParser>();
        services.AddSingleton<ITutorialChildNodeParser, TutorialChildNodeParser>();

        services.AddSingleton<IChoiceDefaultNodeParser, ChoiceDefaultNodeParser>();
        services.AddSingleton<IChoiceTimeLimitNodeParser, ChoiceTimeLimitNodeParser>();
        services.AddSingleton<IChoiceOptionNodeParser, ChoiceOptionNodeParser>();
        services.AddSingleton<IChoiceOptionDisabledTextNodeParser, ChoiceOptionDisabledTextNodeParser>();
        services.AddSingleton<IChoiceOptionEnabledWhenNodeParser, ChoiceOptionEnabledWhenNodeParser>();
        services.AddSingleton<IChoiceOptionIconNodeParser, ChoiceOptionIconNodeParser>();
        services.AddSingleton<IChoiceOptionTextNodeParser, ChoiceOptionTextNodeParser>();

        // Storyboard parsers
        services.AddSingleton<IRootBlockParser, RootBlockParser>();
        services.AddSingleton<IStoryboardParser, StoryboardParser>();

        // Navigators
        services.AddSingleton<IBlockNavigator, BlockNavigator>();
        services.AddSingleton<ISceneNavigator, SceneNavigator>();

        // Transient
        services.AddTransient<IEventManager, EventManager>();

        return services;
    }
}
