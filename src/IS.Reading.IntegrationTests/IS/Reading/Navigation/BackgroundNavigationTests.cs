using IS.Reading.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace IS.Reading.Navigation;

public class BackgroundNavigationTests
{
    private readonly IServiceProvider serviceProvider;

    public BackgroundNavigationTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddISReading();
        serviceProvider = serviceCollection.BuildServiceProvider();        
    }

    [Fact]
    public async Task Test()
    {
        var parser = serviceProvider.GetRequiredService<IStoryboardParser>();
    }
}
