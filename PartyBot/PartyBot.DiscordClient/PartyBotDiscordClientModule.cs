using Microsoft.Extensions.DependencyInjection;
using PartyBot.DiscordClient.Commands;
using PartyBot.DiscordClient.Interfaces;

namespace PartyBot.DiscordClient
{
    public static class PartyBotDiscordClientModule
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<DiscordClientService>();
            services.AddScoped<ICommand, AddBonusStar>();
            services.AddScoped<ICommand, AddMatch>();
        }
    }
}
