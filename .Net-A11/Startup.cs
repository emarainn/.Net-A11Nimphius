using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Dao;
using MovieRepository.Services;

namespace MovieRepository;

/// <summary>
///     Used for registration of new interfaces
/// </summary>
public class Startup
{
    public IServiceProvider ConfigureServices()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddFile("app.log");
        });

        // Add new lines of code here to register any interfaces and concrete services you create
        services.AddTransient<IMainService, MainService>();
        services.AddTransient<IRepository, Repository>();
        services.AddDbContextFactory<MovieContext>();

        return services.BuildServiceProvider();
    }
}
