using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.Versioning;

namespace PartyBot.ScreenParser.Console
{
    [SupportedOSPlatform("Windows")]
    public static class Program
    {
        public static async Task Main()
        {
            await Host
                .CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                    configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    configBuilder.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<ScreenshotService>();
                })
                .RunConsoleAsync();
        }
    }
}


