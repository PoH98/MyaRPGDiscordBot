// See https://aka.ms/new-console-template for more information
using Autofac;
using Discord;
using Discord.WebSocket;
using MyaDiscordBot;
using MyaDiscordBot.Commands;
using MyaDiscordBot.Models;
using Newtonsoft.Json;

var _client = new DiscordSocketClient();

_client.Log += Log;

var builder = new ContainerBuilder();
var commands = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsInterface);
foreach (var command in commands)
{
    var cmd = (ICommand)Activator.CreateInstance(command);
	builder.RegisterInstance<ICommand>(cmd);
}
var settings = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("appsettings.json"));
builder.RegisterInstance<IConfiguration>(settings);
var items = JsonConvert.DeserializeObject<Items>(File.ReadAllText("items.json"));
builder.RegisterInstance<Items>(items);
builder.Build();
Data.Instance.Container = builder.Build();
var token = Data.Instance.Container.Resolve<IConfiguration>().Token;

await _client.LoginAsync(TokenType.Bot, token);
await _client.StartAsync();

await Task.Delay(-1);

Task Log(LogMessage msg)
{
	Console.WriteLine(msg.ToString());
	return Task.CompletedTask;
}