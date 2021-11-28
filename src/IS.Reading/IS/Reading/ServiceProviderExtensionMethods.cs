using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.Parsing;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.NodeParsers;
using IS.Reading.Parsing.TextParsers;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace IS.Reading
{
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
            services.AddSingleton<IProtagonistNodeParser, ProtagonistNodeParser>();
            services.AddSingleton<IPauseNodeParser, PauseNodeParser>();
            services.AddSingleton<IBackgroundLeftNodeParser, BackgroundLeftNodeParser>();
            services.AddSingleton<IBackgroundRightNodeParser, BackgroundRightNodeParser>();
            services.AddSingleton<IBackgroundColorNodeParser, BackgroundColorNodeParser>();
            services.AddSingleton<IBackgroundScrollNodeParser, BackgroundScrollNodeParser>();
            services.AddSingleton<IBackgroundNodeParser, BackgroundNodeParser>();
            services.AddSingleton<IBlockNodeParser, BlockNodeParser>();
            services.AddSingleton<ISpeechNodeParser, SpeechNodeParser>();
            services.AddSingleton<IThoughtNodeParser, ThoughtNodeParser>();
            services.AddSingleton<INarrationNodeParser, NarrationNodeParser>();
            services.AddSingleton<ITutorialNodeParser, TutorialNodeParser>();
            services.AddSingleton<IMoodNodeParser, MoodNodeParser>();
            services.AddSingleton<IPersonNodeParser, PersonNodeParser>();

            // Storyboard parsers
            services.AddSingleton<IRootBlockParser, RootBlockParser>();
            services.AddSingleton<IStoryboardParser, StoryboardParser>();

            // Navigators
            services.AddSingleton<IBlockNavigator, BlockNavigator>();
            services.AddSingleton<ISceneNavigator, SceneNavigator>();

            // Transient
            services.AddTransient<IEventManager, EventManager>();
            services.AddTransient<IParsingContext, ParsingContext>();

            return services;
        }
    }
}
