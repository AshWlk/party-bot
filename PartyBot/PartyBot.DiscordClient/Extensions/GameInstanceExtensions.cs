using Discord;
using Discord.WebSocket;
using PartyBot.Database.Entities;
using PartyBot.DiscordClient.Enums;

namespace PartyBot.DiscordClient.Extensions
{
    internal static class GameInstanceExtensions
    {
        public static async Task<Embed> GetEmbedAsync(this GameInstance gameInstance, DiscordSocketClient client)
        {
            var users = await GetUsersAsync(gameInstance, client);

            var winnerUsername = users.SingleOrDefault(u => u?.Id.ToString() == gameInstance.WinnerUserId)?.Username ?? "CPU";

            var embedBuilder = new EmbedBuilder()
                .AddField("Winner",winnerUsername ?? "CPU")
                .AddField("Game", ((MarioPartyGames)gameInstance.GameId).GetDescription())
                .AddField("Board", ((MarioPartyBoards)gameInstance.BoardId).GetDescription())
                .WithColor(Color.Blue)
                .WithTimestamp(gameInstance.Date);

            foreach (var bonusStar in gameInstance.GameInstanceBonusStars.GroupBy(x => x.BonusStarId))
            {
                var winners = string.Join(", ", users.Where(x => IsBonusStarWinner(x, bonusStar)).Select(x => x?.Username ?? "CPU"));
                var bonusStarName = ((MarioPartyBonusStars)bonusStar.Key).GetDescription();

                embedBuilder.AddField(bonusStarName, winners);
            }

            if (winnerUsername != null)
            {
                embedBuilder.WithAuthor(winnerUsername);
            }

            return embedBuilder.Build();
        }

        private static async Task<IEnumerable<IUser?>> GetUsersAsync(GameInstance gameInstance, DiscordSocketClient client)
        {
            var userIds = new List<string?>
            {
                gameInstance.WinnerUserId
            };

            userIds.AddRange(gameInstance.GameInstanceBonusStars.Select(g => g.WinnerUserId).Distinct());

            var users = new List<IUser?>();

            foreach (var userId in userIds.Distinct())
            {
                users.Add(await GetUserAsync(userId, client));
            }

            return users;
        }

        private static async Task<IUser?> GetUserAsync(string? userId, DiscordSocketClient client)
        {
            if (userId == null)
            {
                return null;
            }

            return await client.GetUserAsync(ulong.Parse(userId));
        }

        private static bool IsBonusStarWinner(IUser? user, IEnumerable<GameInstanceBonusStar> bonusStars)
        {
            return bonusStars.Select(x => x.WinnerUserId).Contains(user?.Id.ToString());
        }
    }
}
