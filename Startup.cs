using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using personalSite.Interfaces;
using personalSite.Services;

[assembly: FunctionsStartup(typeof(personalSite.Startup))]

namespace personalSite
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ICosmosDb, CosmosDb>();
        }
    }
} 