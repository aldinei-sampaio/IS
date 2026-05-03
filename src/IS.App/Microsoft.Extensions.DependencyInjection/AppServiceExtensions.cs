using IS.App.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class AppServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IAssetManager, AssetManager>();
        return services;
    }
}
