using Discord;
using Discord.WebSocket;
using PartyBot.Database;
using PartyBot.DiscordClient.Enums;
using PartyBot.DiscordClient.Extensions;
using PartyBot.DiscordClient.Interfaces;

namespace PartyBot.DiscordClient.Commands
{
    public class AddMatch : ICommand
    {
        private readonly PartyBotDbContext _dbContext;
        private readonly DiscordSocketClient _client;

        public AddMatch(PartyBotDbContext dbContext, DiscordSocketClient client)
        {
            this._dbContext = dbContext;
            this._client = client;
        }

        public string Name => "add-match";

        public SlashCommandBuilder GetBuilder()
        {
            return new SlashCommandBuilder()
                .WithName("add-match")
                .WithDescription("Logs the results of a match")
                .AddOption("winner", ApplicationCommandOptionType.User, "The winner of the match", isRequired: false)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("game")
                    .WithDescription("The version of Mario Party that was played")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoicesFromEnum<MarioPartyGames>())
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("board")
                    .WithDescription("The board that was played")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoicesFromEnum<MarioPartyBoards>());
        }

        public async Task<Action<MessageProperties>> HandleAsync(SocketSlashCommand command)
        {
            var winningUser = (SocketGuildUser?)command.Data.Options.SingleOrDefault(o => o.Name == "winner")?.Value;

            var gameInstance = await this._dbContext.GameInstsances.AddAsync(new Database.Entities.GameInstance
            {
                WinnerUserId = winningUser?.Id.ToString(),
                GameId = (long)command.Data.Options.Single(o => o.Name == "game").Value,
                BoardId = (long)command.Data.Options.Single(o => o.Name == "board").Value,
                Date = DateTimeOffset.UtcNow
            });

            return async (message) =>
            {
                message.Embed = (await gameInstance.Entity.GetEmbedBuilder(winningUser, this._client)).Build();
            };
        }
    }
}
