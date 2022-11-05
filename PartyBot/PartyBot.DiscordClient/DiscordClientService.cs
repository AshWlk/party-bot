using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PartyBot.Database;
using PartyBot.DiscordClient.Extensions;
using PartyBot.DiscordClient.Interfaces;

namespace PartyBot.DiscordClient
{
    internal class DiscordClientService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DiscordClientService> _logger;

        private DiscordSocketClient _discordSocketClient;

        public DiscordClientService(IServiceProvider serviceProvider, ILogger<DiscordClientService> logger)
        {
            this._serviceProvider = serviceProvider;
            this._logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this._discordSocketClient = new();
            await this._discordSocketClient.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("$BOT_TOKEN"));
            await this._discordSocketClient.StartAsync();

            this._discordSocketClient.Log += this.DiscordSocketClient_Log;
            this._discordSocketClient.SlashCommandExecuted += this.DiscordSocketClient_SlashCommandExecuted;
            this._discordSocketClient.Ready += DiscordSocketClient_Ready;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await this._discordSocketClient.LogoutAsync();
            await this._discordSocketClient.StopAsync();
            await this._discordSocketClient.DisposeAsync();
        }

        private async Task DiscordSocketClient_SlashCommandExecuted(SocketSlashCommand arg)
        {
            await arg.DeferAsync();
            using var scope = this._serviceProvider.CreateAsyncScope();

            var command = scope.ServiceProvider.GetServices<ICommand>().Single(c => c.Name == arg.CommandName);
            var dbContext = scope.ServiceProvider.GetRequiredService<PartyBotDbContext>();

            var action = await command.HandleAsync(arg);
            await arg.ModifyOriginalResponseAsync(action);
            await dbContext.SaveChangesAsync();
        }

        private async Task DiscordSocketClient_Ready()
        {
            using var scope = this._serviceProvider.CreateAsyncScope();
            var commands = scope.ServiceProvider.GetServices<ICommand>();

            foreach (var guild in _discordSocketClient.Guilds)
            {
                await guild.BulkOverwriteApplicationCommandAsync(commands.Select(c => c.GetBuilder().Build()).ToArray());
            }
        }

        private Task DiscordSocketClient_Log(LogMessage arg)
        {
            this._logger.Log(arg.GetLogLevel(), "{DiscordClientMessage}", arg.Message);
            return Task.CompletedTask;
        }
    }
}
