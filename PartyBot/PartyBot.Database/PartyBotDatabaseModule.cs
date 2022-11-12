using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PartyBot.Database
{
    public static class PartyBotDatabaseModule
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PartyBotDbContext>(options =>
            {
                options.UseSqlServer(@"Server=.;Database=PartyBot5;Integrated Security=True;TrustServerCertificate=true");
            });
        }
    }
}
