﻿using IS.Reading.Parsing;
using IS.Reading.Parsing.Attributes;
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
            services.AddTransient<IElementParser, ElementParser>();

            return services;
        }
    }
}