using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using PartyBot.Database;
using PartyBot.DiscordClient.Commands;
using PartyBot.DiscordClient.Interfaces;

var _context = new PartyBotDbContext(options => options.UseSqlServer(@"Server=.;Database=PartyBot5;Integrated Security=True;"));
_context.Database.EnsureCreated();

var _client = new DiscordSocketClient();

var _commands = new ICommand[]
{
    new AddMatch(_context),
    new AddBonusStar(_context)
};

_client.Log += Client_Log;
_client.SlashCommandExecuted += Client_SlashCommandExecuted;
_client.Ready += Client_Ready;

await _client.LoginAsync(Discord.TokenType.Bot, Environment.GetEnvironmentVariable("$BOT_TOKEN"));
await _client.StartAsync();
await Task.Delay(-1);

async Task Client_SlashCommandExecuted(SocketSlashCommand command)
{
    await command.DeferAsync(true);
    var action = await _commands.Single(c => c.Name == command.CommandName).HandleAsync(command);
    await _context.SaveChangesAsync();
    await command.ModifyOriginalResponseAsync(action);
}

static Task Client_Log(LogMessage arg)
{
    Console.WriteLine(arg.ToString());
    return Task.CompletedTask;
}

async Task Client_Ready()
{
    foreach (var guild in _client.Guilds)
    {
        await guild.BulkOverwriteApplicationCommandAsync(_commands.Select(c => c.GetBuilder().Build()).ToArray());
    }
}

