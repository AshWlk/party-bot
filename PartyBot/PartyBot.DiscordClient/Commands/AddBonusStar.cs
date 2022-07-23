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

        public AddBonusStar(PartyBotDbContext dbContext)
        {
            this._dbContext = dbContext;
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
                .OrderByDescending(g => g.Date)
                .FirstAsync();

            var gameInstanceBonusStar = await this._dbContext.GameInstanceBonusStars.AddAsync(new Database.Entities.GameInstanceBonusStar
            {
                WinnerUserId = ((SocketGuildUser)command.Data.Options.Single(o => o.Name == "winner").Value).Id,
                BonusStarId = (int)command.Data.Options.Single(o => o.Name == "bonus-star").Value,
                GameInstanceId = gameInstance.Id
            });

            return (message) =>
            {
                message.Content = $"Bonus Star: {gameInstanceBonusStar.Entity.BonusStarId}, Match ID: {gameInstanceBonusStar.Entity.GameInstanceId}";
            };
        }
    }
}
