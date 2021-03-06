using Microsoft.EntityFrameworkCore;
using PartyBot.Database.Entities;
using PartyBot.Database.Helpers;

namespace PartyBot.Database
{
    public class PartyBotDbContext : DbContext
    {
        public DbSet<Board> Matches { get; set; }

        public DbSet<GameInstance> GameInstsances { get; set; }

        public DbSet<GameInstanceBonusStar> GameInstanceBonusStars { get; set; }

        public DbSet<Board> Boards { get; set; }

        public DbSet<BonusStar> BonusStars { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<GameBonusStar> GameBonusStars { get; set;}

        public PartyBotDbContext(Action<DbContextOptionsBuilder<PartyBotDbContext>> options) : base(DbContextOptionsFactory.Build(options))
        {
        }
    }
}