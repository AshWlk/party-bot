using Discord;
using Discord.WebSocket;
using PartyBot.Database.Entities;
using PartyBot.DiscordClient.Enums;

namespace PartyBot.DiscordClient.Extensions
{
    internal static class GameInstanceExtensions
    {
        public static async Task<EmbedBuilder> GetEmbedBuilder(this GameInstance gameInstance, SocketGuildUser? user, DiscordSocketClient client)
        {
            var embedBuilder = new EmbedBuilder()
                .AddField("Winner", user?.DisplayName)
                .AddField("Game", ((MarioPartyGames)gameInstance.GameId).GetDescription())
                .AddField("Board", ((MarioPartyBoards)gameInstance.BoardId).GetDescription())
                .WithColor(Color.Blue)
                .WithTimestamp(gameInstance.Date);

            foreach (var bonusStarWinner in gameInstance.GameInstanceBonusStars.GroupBy(x => x.WinnerUserId))
            {
                foreach (var bonusStar in bonusStarWinner)
                {
                    embedBuilder.AddField(((MarioPartyBonusStars)bonusStar.BonusStarId).GetDescription(), await GetUserNameFromId(bonusStarWinner.Key, client));
                }
            }

            if (user != null)
            {
                embedBuilder.WithAuthor(user);
            }

            return embedBuilder;
        }

        private static async Task<string> GetUserNameFromId(string? userId, DiscordSocketClient client)
        {
            if (userId == null)
            {
                return "CPU";
            }

            return (await client.GetUserAsync(ulong.Parse(userId))).Username;
        }
    }
}
