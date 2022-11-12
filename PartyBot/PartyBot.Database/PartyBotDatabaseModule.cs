using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PartyBot.Database
{
    public static class PartyBotDatabaseModule
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PartyBotDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Database"));
            });
        }
    }
}
