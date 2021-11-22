using IS.Reading.Navigation;
using IS.Reading.Parsing;
using IS.Reading.Parsing.AttributeParsers;
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
            services.AddSingleton<IConditionParser, ConditionParser>();
            services.AddSingleton<IWhenAttributeParser, WhenAttributeParser>();
            services.AddSingleton<IWhileAttributeParser, WhileAttributeParser>();            
            services.AddSingleton<IElementParser, ElementParser>();

            services.AddSingleton<IColorTextParser, ColorTextParser>();
            services.AddSingleton<IBackgroundImageTextParser, BackgroundImageTextParser>();

            services.AddSingleton<IBlockNavigator, BlockNavigator>();

            return services;
        }
    }
}
