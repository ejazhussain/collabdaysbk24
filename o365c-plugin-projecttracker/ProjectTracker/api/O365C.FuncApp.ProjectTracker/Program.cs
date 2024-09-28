using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using O365C.FuncApp.ProjectTracker.Models;
using O365C.FuncApp.ProjectTracker.Services;

AzureFunctionSettings azureFunctionSettings = null;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        

        // Add our global configuration instance
        services.AddSingleton(options =>
        {
            var configuration = context.Configuration;
            azureFunctionSettings = new AzureFunctionSettings();
            configuration.Bind(azureFunctionSettings);
            return configuration;
        });

        // Add our configuration class                
        services.AddSingleton(options => { return azureFunctionSettings; });

        //Add SQL service
        services.AddSingleton<ISQLService, SQLService>();


    })
    .Build();

host.Run();
