using Discord;
using Discord.WebSocket;
using PartyBot.DiscordClient.Enums;
using PartyBot.DiscordClient.Extensions;

var _client = new DiscordSocketClient();

_client.Log += Client_Log;
_client.SlashCommandExecuted += Client_SlashCommandExecuted;
_client.Ready += Client_Ready;

await _client.LoginAsync(Discord.TokenType.Bot, "");
await _client.StartAsync();
await Task.Delay(-1);

static async Task Client_SlashCommandExecuted(SocketSlashCommand command)
{
    await command.DeferAsync(true);
    await Task.Delay(1000);
    await command.ModifyOriginalResponseAsync((message) =>
    {
        message.Content = "Hello world";
    });
}

static Task Client_Log(LogMessage arg)
{
    Console.WriteLine(arg.ToString());
    return Task.CompletedTask;
}

async Task Client_Ready()
{
    var command = new SlashCommandBuilder()
        .WithName("add-match")
        .WithDescription("Logs the results of a match")
        .AddOption("winner", ApplicationCommandOptionType.User, "The winner of the match", isRequired: true)
        .AddOption(new SlashCommandOptionBuilder()
            .WithName("game")
            .WithDescription("The version of Mario Party that was played")
            .WithRequired(true)
            .WithType(ApplicationCommandOptionType.Integer)
            .AddChoicesFromEnum<MarioPartyGames>())
        .Build();



    await _client.CreateGlobalApplicationCommandAsync(command);
}

