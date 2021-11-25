﻿using IS.Reading.Navigation;
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
        public static IServiceCollection AddParsing(this IServiceCollection services)
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

            // Node parsers
            services.AddSingleton<IPauseNodeParser, PauseNodeParser>();
            services.AddSingleton<IBackgroundLeftNodeParser, BackgroundLeftNodeParser>();
            services.AddSingleton<IBackgroundRightNodeParser, BackgroundRightNodeParser>();
            services.AddSingleton<IBackgroundColorNodeParser, BackgroundColorNodeParser>();
            services.AddSingleton<IBackgroundScrollNodeParser, BackgroundScrollNodeParser>();
            services.AddSingleton<IBackgroundNodeParser, BackgroundNodeParser>();

            // Navigators
            services.AddSingleton<IBlockNavigator, BlockNavigator>();
            services.AddSingleton<ISceneNavigator, SceneNavigator>();

            return services;
        }
    }
}
