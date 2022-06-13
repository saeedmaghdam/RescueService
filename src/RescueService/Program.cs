using RescueService;
using Serilog;

var Configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext())
    .ConfigureServices(services =>
    {
        services.AddOptions();
        services.Configure<ApplicationOptions>(Configuration);

        services.AddSingleton<InfraService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
