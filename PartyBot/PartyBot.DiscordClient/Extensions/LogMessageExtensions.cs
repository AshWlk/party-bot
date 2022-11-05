using Discord;
using Microsoft.Extensions.Logging;

namespace PartyBot.DiscordClient.Extensions
{
    internal static class LogMessageExtensions
    {
        public static LogLevel GetLogLevel(this LogMessage logMessage)
        {
            return logMessage.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Debug,
                LogSeverity.Debug => LogLevel.Trace,
                _ => LogLevel.None
            };
        }
    }
}
