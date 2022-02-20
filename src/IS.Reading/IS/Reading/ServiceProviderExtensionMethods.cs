using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.Parsing;
using IS.Reading.Parsing.ConditionParsers;
using IS.Reading.Parsing.NodeParsers;
using IS.Reading.Parsing.NodeParsers.BackgroundParsers;
using IS.Reading.Parsing.NodeParsers.ChoiceParsers;
using IS.Reading.Parsing.NodeParsers.PersonParsers;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.State;
using IS.Reading.Variables;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace IS.Reading;

[ExcludeFromCodeCoverage]
public static class ServiceProviderExtensionMethods
{
    public static IServiceCollection AddISReading(this IServiceCollection services)
    {
        // Auxiliar
        services.AddSingleton<IRandomizer, Randomizer>();
        services.AddSingleton<IBlockStateFactory, BlockStateFactory>();

        // Auxiliar parsers
        services.AddSingleton<IWordReaderFactory, WordReaderFactory>();
        services.AddSingleton<IConditionParser, ConditionParser>();
        services.AddSingleton<IElementParser, ElementParser>();
        services.AddSingleton<IVarSetParser, VarSetParser>();
        services.AddSingleton<ITextSourceParser, TextSourceParser>();

        // Text parsers
        services.AddSingleton<IColorArgumentParser, ColorArgumentParser>();
        services.AddSingleton<IImageArgumentParser, ImageArgumentParser>();
        services.AddSingleton<IIntegerArgumentParser, IntegerArgumentParser>();
        services.AddSingleton<INameArgumentParser, NameArgumentParser>();

        // Node parsers
        services.AddSingleton<IMusicNodeParser, MusicNodeParser>();
        services.AddSingleton<IMainCharacterNodeParser, MainCharacterNodeParser>();
        services.AddSingleton<IPauseNodeParser, PauseNodeParser>();
        services.AddSingleton<IBlockNodeParser, BlockNodeParser>();
        services.AddSingleton<IMoodNodeParser, MoodNodeParser>();
        services.AddSingleton<IPersonNodeParser, PersonNodeParser>();
        services.AddSingleton<ISetNodeParser, SetNodeParser>();

        services.AddSingleton<IBackgroundNodeParser, BackgroundNodeParser>();
        services.AddSingleton<IBackgroundLeftNodeParser, BackgroundLeftNodeParser>();
        services.AddSingleton<IBackgroundRightNodeParser, BackgroundRightNodeParser>();
        services.AddSingleton<IBackgroundColorNodeParser, BackgroundColorNodeParser>();
        services.AddSingleton<IBackgroundScrollNodeParser, BackgroundScrollNodeParser>();

        services.AddSingleton<ISpeechNodeParser, SpeechNodeParser>();
        services.AddSingleton<ISpeechChildNodeParser, SpeechChildNodeParser>();
        services.AddSingleton<IThoughtNodeParser, ThoughtNodeParser>();
        services.AddSingleton<IThoughtChildNodeParser, ThoughtChildNodeParser>();
        services.AddSingleton<INarrationNodeParser, NarrationNodeParser>();
        services.AddSingleton<INarrationChildNodeParser, NarrationChildNodeParser>();
        services.AddSingleton<ITutorialNodeParser, TutorialNodeParser>();
        services.AddSingleton<ITutorialChildNodeParser, TutorialChildNodeParser>();

        services.AddSingleton<IChoiceNodeParser, ChoiceNodeParser>();
        services.AddSingleton<IChoiceDefaultNodeParser, ChoiceDefaultNodeParser>();
        services.AddSingleton<IChoiceTimeLimitNodeParser, ChoiceTimeLimitNodeParser>();
        services.AddSingleton<IChoiceRandomOrderNodeParser, ChoiceRandomOrderNodeParser>();
        services.AddSingleton<IChoiceIfNodeParser, ChoiceIfNodeParser>();

        services.AddSingleton<IChoiceOptionNodeParser, ChoiceOptionNodeParser>();
        services.AddSingleton<IChoiceOptionDisabledNodeParser, ChoiceOptionDisabledNodeParser>();
        services.AddSingleton<IChoiceOptionIconNodeParser, ChoiceOptionIconNodeParser>();
        services.AddSingleton<IChoiceOptionTextNodeParser, ChoiceOptionTextNodeParser>();
        services.AddSingleton<IChoiceOptionTipNodeParser, ChoiceOptionTipNodeParser>();
        services.AddSingleton<IChoiceOptionIfNodeParser, ChoiceOptionIfNodeParser>();

        // Storyboard parsers
        services.AddSingleton<IRootBlockParser, RootBlockParser>();
        services.AddSingleton<IStoryboardParser, StoryboardParser>();

        // Navigators
        services.AddSingleton<IBlockNavigator, BlockNavigator>();
        services.AddSingleton<ISceneNavigator, SceneNavigator>();

        // Transient
        services.AddTransient<IEventManager, EventManager>();
        services.AddTransient<IBlockFactory, BlockFactory>();
        services.AddTransient<IParsingSceneContext, ParsingSceneContext>();
        services.AddTransient<IParsingContext, ParsingContext>();
        services.AddTransient<INavigationState, NavigationState>();
        services.AddTransient<IVariableDictionary, VariableDictionary>();
        services.AddTransient<IBlockState, BlockState>();
        services.AddTransient<IBlockIterationState, BlockIterationState>();
        services.AddTransient<IBlockStateDictionary, BlockStateDictionary>();

        return services;
    }
}
