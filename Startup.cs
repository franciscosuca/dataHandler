using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using personalSite.Interfaces;
using personalSite.Services;
// This attribute tells Azure Functions to use this Startup class for dependency injection configuration
[assembly: FunctionsStartup(typeof(personalSite.Startup))]

namespace personalSite;
public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddScoped<IExperienceHandler, ExperienceHandler>();
        builder.Services.AddScoped<ICosmosDb, CosmosDb>();
        builder.Services.AddScoped<BlobService>();
        builder.Services.AddOptions<Configuration>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("CosmosDb").Bind(settings);
            });
    }
}
