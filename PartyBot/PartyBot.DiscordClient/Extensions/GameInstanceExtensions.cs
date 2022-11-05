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
                .AddField("Winner", user?.Nickname ?? "CPU")
                .AddField("Game", ((MarioPartyGames)gameInstance.GameId).GetDescription())
                .AddField("Board", ((MarioPartyBoards)gameInstance.BoardId).GetDescription())
                .WithColor(Color.Blue)
                .WithTimestamp(gameInstance.Date);

            var bonusStarWinnerUserIds = gameInstance.GameInstanceBonusStars.Select(x => x.WinnerUserId).Distinct();
            var bonusStarWinners = new List<IUser?>();

            foreach (var bonusStarWinnerUserId in bonusStarWinnerUserIds)
            {
                bonusStarWinners.Add(await GetUser(bonusStarWinnerUserId, client));
            }

            foreach (var bonusStar in gameInstance.GameInstanceBonusStars.GroupBy(x => x.BonusStarId))
            {
                var winners = string.Join(", ", bonusStarWinners.Where(x => BonusStarWinnerPredicate(x, bonusStar)).Select(x => x?.Username ?? "CPU"));
                var bonusStarName = ((MarioPartyBonusStars)bonusStar.Key).GetDescription();

                embedBuilder.AddField(bonusStarName, winners);
            }

            if (user != null)
            {
                embedBuilder.WithAuthor(user);
            }

            return embedBuilder;
        }

        private static async Task<IUser?> GetUser(string? userId, DiscordSocketClient client)
        {
            if (userId == null)
            {
                return null;
            }

            return (await client.GetUserAsync(ulong.Parse(userId)));
        }

        private static bool BonusStarWinnerPredicate(IUser? user, IEnumerable<GameInstanceBonusStar> bonusStars)
        {
            return bonusStars.Select(x => x.WinnerUserId).Contains(user?.Id.ToString());
        }
    }
}
