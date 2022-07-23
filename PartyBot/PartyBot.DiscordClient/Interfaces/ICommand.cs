using Discord;
using Discord.WebSocket;

namespace PartyBot.DiscordClient.Interfaces
{
    public interface ICommand
    {
        string Name { get; }

        Task<Action<MessageProperties>> HandleAsync(SocketSlashCommand command);

        SlashCommandBuilder GetBuilder();
    }
}
