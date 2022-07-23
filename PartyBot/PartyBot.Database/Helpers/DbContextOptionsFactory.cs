using Microsoft.EntityFrameworkCore;

namespace PartyBot.Database.Helpers
{
    internal static class DbContextOptionsFactory
    {
        public static DbContextOptions Build<TContext>(Action<DbContextOptionsBuilder<TContext>> optionsAction)
            where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsAction(optionsBuilder);
            return optionsBuilder.Options;
        }
    }
}
