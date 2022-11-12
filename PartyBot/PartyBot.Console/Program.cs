using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PartyBot.Database;
using PartyBot.DiscordClient;

await Host
    .CreateDefaultBuilder()
    .UseConsoleLifetime()
    .ConfigureAppConfiguration(configBuilder =>
    {
        configBuilder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
        configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        configBuilder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        PartyBotDatabaseModule.ConfigureServices(services, configuration);
        PartyBotDiscordClientModule.ConfigureServices(services);
    })
    .RunConsoleAsync();

