// See https://aka.ms/new-console-template for more information
using Autofac;
using Discord;
using Discord.WebSocket;
using MyaDiscordBot;
using MyaDiscordBot.ButtonEvent;
using MyaDiscordBot.Commands;
using MyaDiscordBot.GameLogic.Events;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using System.Reflection;

//Create DC Bot Client//
var _client = new DiscordSocketClient();
StdSchedulerFactory factory = new StdSchedulerFactory();
_client.Log += Log;
IScheduler scheduler = await factory.GetScheduler();
IJobDetail job = JobBuilder.Create<BossJob>()
    .WithIdentity("job", "group")
    .Build();

// Trigger the job to run now, and then repeat every 10 seconds
ITrigger trigger = TriggerBuilder.Create()
    .WithIdentity("trigger", "group")
    .StartAt(DateTimeOffset.Now.AddDays(7 - (int)DateTime.Now.DayOfWeek))
    .WithDailyTimeIntervalSchedule(x => x.WithIntervalInHours(1))
    .Build();
await scheduler.ScheduleJob(job, trigger);
await scheduler.Start();
var commandHandler = new CommandHandler(_client);
await commandHandler.InstallCommandsAsync();
//==============================================================================//
//Register DI//
var builder = new ContainerBuilder();
//Load all commands
var commands = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsInterface);
var buttons = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IButtonHandler).IsAssignableFrom(p) && !p.IsInterface);
var rndEvents = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IRandomEvent).IsAssignableFrom(p) && !p.IsInterface);
foreach (var command in commands)
{
    MethodInfo method = typeof(RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
    method = method.MakeGenericMethod(command);
    method.Invoke(builder, new object[1] { builder });
}
foreach (var button in buttons)
{
    MethodInfo method = typeof(RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
    method = method.MakeGenericMethod(button);
    method.Invoke(builder, new object[1] { builder });
}
foreach (var e in rndEvents)
{
    MethodInfo method = typeof(RegistrationExtensions).GetMethods().Where(x => x.Name == "RegisterType" && x.IsGenericMethod).Last();
    method = method.MakeGenericMethod(e);
    method.Invoke(builder, new object[1] { builder });
}
//Load all configs
var settings = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config\\appsettings.json"));
builder.RegisterInstance<IConfiguration>(settings);
var items = JsonConvert.DeserializeObject<Items>(File.ReadAllText("config\\items.json"));
builder.RegisterInstance<Items>(items);
var enemy = JsonConvert.DeserializeObject<Enemies>(File.ReadAllText("config\\enemy.json"));
builder.RegisterInstance<Enemies>(enemy);
builder.RegisterInstance<DiscordSocketClient>(_client);
//Load all Services
builder.RegisterType<PlayerService>().As<IPlayerService>();
builder.RegisterType<MapService>().As<IMapService>();
builder.RegisterType<SpawnerService>().As<ISpawnerService>();
builder.RegisterType<BattleService>().As<IBattleService>();
builder.RegisterType<ItemService>().As<IItemService>();
builder.RegisterType<EventService>().As<IEventService>();
builder.RegisterType<SettingService>().As<ISettingService>();
builder.RegisterType<BossService>().As<IBossService>();
//=============================================================================//
//Start Bot//
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