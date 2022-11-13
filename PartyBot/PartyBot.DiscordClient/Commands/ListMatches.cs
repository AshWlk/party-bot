using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using PartyBot.Database;
using PartyBot.DiscordClient.Extensions;
using PartyBot.DiscordClient.Interfaces;

namespace PartyBot.DiscordClient.Commands
{
    public class ListMatches : ICommand
    {
        private readonly PartyBotDbContext _dbContext;
        private readonly DiscordSocketClient _client;

        public ListMatches(PartyBotDbContext dbContext, DiscordSocketClient client)
        {
            this._dbContext = dbContext;
            this._client = client;
        }

        public string Name => "list-matches";

        public SlashCommandBuilder GetBuilder()
        {
            return new SlashCommandBuilder()
                .WithName(this.Name)
                .WithDescription("Lists the most recent matches");
        }

        public async Task<Action<MessageProperties>> HandleAsync(SocketSlashCommand command)
        {
            var gameInstances = await this._dbContext.GameInstsances
                .Include(x => x.GameInstanceBonusStars)
                .OrderByDescending(g => g.Date)
                .ToListAsync();

            var embeds = new List<Embed>();

            foreach (var gameInstance in gameInstances)
            {
                embeds.Add(await gameInstance.GetEmbedAsync(this._client));
            }

            return (message) =>
            {
                message.Embeds = embeds.ToArray();
            };
        }
    }
}
