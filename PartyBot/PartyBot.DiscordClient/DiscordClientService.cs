using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly DiscordSocketClient _client;

        public DiscordClientService(IServiceProvider serviceProvider, ILogger<DiscordClientService> logger, IConfiguration configuration, DiscordSocketClient client)
        {
            this._serviceProvider = serviceProvider;
            this._logger = logger;
            this._configuration = configuration;
            this._client = client;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = this._serviceProvider.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<PartyBotDbContext>();
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);

            await this._client.LoginAsync(TokenType.Bot, this._configuration.GetRequiredSection("DiscordClientToken").Value);
            await this._client.StartAsync();

            this._client.Log += this.DiscordSocketClient_Log;
            this._client.SlashCommandExecuted += this.DiscordSocketClient_SlashCommandExecuted;
            this._client.Ready += DiscordSocketClient_Ready;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await this._client.LogoutAsync();
            await this._client.StopAsync();
            await this._client.DisposeAsync();
        }

        private async Task DiscordSocketClient_SlashCommandExecuted(SocketSlashCommand arg)
        {
            await arg.DeferAsync(bool.Parse(this._configuration.GetSection("UseEphemeralResponse")?.Value ?? false.ToString()));

            try
            {
                using var scope = this._serviceProvider.CreateAsyncScope();

                var command = scope.ServiceProvider.GetServices<ICommand>().Single(c => c.Name == arg.CommandName);
                var dbContext = scope.ServiceProvider.GetRequiredService<PartyBotDbContext>();

                var action = await command.HandleAsync(arg);
                await arg.ModifyOriginalResponseAsync(action);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "{Exception}", ex.Message);
                await arg.ModifyOriginalResponseAsync(message =>
                {
                    message.Content = "PartyBot encountered an error. Please try again later.";
                });
            }
        }

        private async Task DiscordSocketClient_Ready()
        {
            using var scope = this._serviceProvider.CreateAsyncScope();
            var commands = scope.ServiceProvider.GetServices<ICommand>();

            foreach (var guild in _client.Guilds)
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
