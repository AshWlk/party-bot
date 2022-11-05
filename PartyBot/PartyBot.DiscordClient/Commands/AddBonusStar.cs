using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using PartyBot.Database;
using PartyBot.DiscordClient.Enums;
using PartyBot.DiscordClient.Extensions;
using PartyBot.DiscordClient.Interfaces;

namespace PartyBot.DiscordClient.Commands
{
    public class AddBonusStar : ICommand
    {
        private readonly PartyBotDbContext _dbContext;
        private readonly DiscordSocketClient _client;

        public AddBonusStar(PartyBotDbContext dbContext, DiscordSocketClient client)
        {
            this._dbContext = dbContext;
            this._client = client;
        }

        public string Name => "add-bonus-star";

        public SlashCommandBuilder GetBuilder()
        {
            return new SlashCommandBuilder()
                .WithName(this.Name)
                .WithDescription("Adds a bonus star to the most recent match")
                .AddOption("winner", ApplicationCommandOptionType.User, "The winner of the bonus star", isRequired: true)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("bonus-star")
                    .WithDescription("The bonus star that was awarded")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Integer)
                    .AddChoicesFromEnum<MarioPartyBonusStars>());
        }

        public async Task<Action<MessageProperties>> HandleAsync(SocketSlashCommand command)
        {
            var gameInstance = await this._dbContext.GameInstsances
                .Include(x => x.GameInstanceBonusStars)
                .OrderByDescending(g => g.Date)
                .FirstAsync();

            var winningUser = (SocketGuildUser)command.Data.Options.Single(o => o.Name == "winner").Value;

            gameInstance.GameInstanceBonusStars.Add(new()
            {
                WinnerUserId = winningUser?.Id.ToString(),
                BonusStarId = (long)command.Data.Options.Single(o => o.Name == "bonus-star").Value,
                GameInstanceId = gameInstance.Id
            });

            return async (message) =>
            {
                message.Embed = (await gameInstance.GetEmbedBuilder(winningUser, this._client)).Build();
            };
        }
    }
}
