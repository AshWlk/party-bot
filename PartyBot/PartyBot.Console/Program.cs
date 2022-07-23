using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using PartyBot.Database;
using PartyBot.DiscordClient.Commands;

var _context = new PartyBotDbContext(options => options.UseSqlServer(@"Server=.;Database=PartyBot5;Integrated Security=True;"));
_context.Database.EnsureCreated();

var _client = new DiscordSocketClient();

var _commands = new[]
{
    new AddMatch(_context)
};

_client.Log += Client_Log;
_client.SlashCommandExecuted += Client_SlashCommandExecuted;
_client.Ready += Client_Ready;

await _client.LoginAsync(Discord.TokenType.Bot, "");
await _client.StartAsync();
await Task.Delay(-1);

async Task Client_SlashCommandExecuted(SocketSlashCommand command)
{
    await command.DeferAsync(true);
    var action = await _commands.Single(c => c.Name == command.CommandName).HandleAsync(command);
    await command.ModifyOriginalResponseAsync(action);
}

static Task Client_Log(LogMessage arg)
{
    Console.WriteLine(arg.ToString());
    return Task.CompletedTask;
}

async Task Client_Ready()
{
    foreach (var command in _commands)
    {
        await _client.CreateGlobalApplicationCommandAsync(command.GetBuilder().Build());
    }
}

