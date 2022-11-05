using Microsoft.Extensions.Hosting;
using PartyBot.Database;
using PartyBot.DiscordClient;

await Host
    .CreateDefaultBuilder()
    .UseConsoleLifetime()
    .ConfigureServices(services =>
    {
        PartyBotDatabaseModule.ConfigureServices(services);
        PartyBotDiscordClientModule.ConfigureServices(services);
    })
    .RunConsoleAsync();

