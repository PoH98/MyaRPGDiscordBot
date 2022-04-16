// See https://aka.ms/new-console-template for more information
using Autofac;
using Discord;
using Discord.WebSocket;
using MyaDiscordBot;
using MyaDiscordBot.Extension;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
//==============================================================================//
//Create DC Bot Client//
var _client = new DiscordSocketClient();
_client.Log += Log;
//==============================================================================//
//Scheduller Register//
LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
StdSchedulerFactory factory = new StdSchedulerFactory();
IScheduler scheduler = await factory.GetScheduler();
IJobDetail job = JobBuilder.Create<BossJob>()
    .WithIdentity("job", "group")
    .Build();
ITrigger trigger = TriggerBuilder.Create()
    .WithIdentity("trigger", "group")
    .StartNow().WithDailyTimeIntervalSchedule(x => x.WithIntervalInHours(1))
    .Build();
await scheduler.ScheduleJob(job, trigger);
//==============================================================================//
//Register DI//
var builder = new ContainerBuilder();
//Load all commands
builder.RegisterInterfaces();
//Register Socket Client
builder.RegisterInstance<DiscordSocketClient>(_client);
//Load all configs
builder.RegisterSettingsConfig(_client);
//Load all Services
builder.RegisterMyaBotService();
//=============================================================================//
//Build container//
Data.Instance.Container = builder.Build();
var token = Data.Instance.Container.Resolve<IConfiguration>().Token;
//==============================================================================//
//Start bot//
await _client.LoginAsync(TokenType.Bot, token);
await scheduler.Start();
await _client.StartAsync();

await Task.Delay(-1);

Task Log(LogMessage msg)
{
    Console.WriteLine(msg.ToString());
    return Task.CompletedTask;
}

class ConsoleLogProvider : ILogProvider
{
    public Logger GetLogger(string name)
    {
        return (level, func, exception, parameters) =>
        {
            if (level >= LogLevel.Info && func != null)
            {
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
            }
            return true;
        };
    }

    public IDisposable OpenNestedContext(string message)
    {
        throw new NotImplementedException();
    }

    public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
    {
        throw new NotImplementedException();
    }
}