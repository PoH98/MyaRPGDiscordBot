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
using System.Diagnostics;
using System.Reflection;

Process[] instances = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
int procId = Process.GetCurrentProcess().Id;
if (instances.Length > 1)
{
    foreach (Process i in instances)
    {
        if (i.Id != procId)
        {
            i.Kill();
        }
    }
}
//==============================================================================//
//Create DC Bot Client//
DiscordSocketClient _client = new(new DiscordSocketConfig
{
    GatewayIntents = GatewayIntents.GuildMembers | GatewayIntents.GuildMessages | GatewayIntents.Guilds | GatewayIntents.GuildIntegrations
});
_client.Log += Log;
//==============================================================================//
//Scheduller Register//
LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
StdSchedulerFactory factory = new();
IScheduler scheduler = await factory.GetScheduler();
IJobDetail job = JobBuilder.Create<BossJob>()
    .Build();
ITrigger trigger = TriggerBuilder.Create()
    .StartNow().WithDailyTimeIntervalSchedule(x => x.WithIntervalInHours(1))
    .Build();
await scheduler.ScheduleJob(job, trigger);
job = JobBuilder.Create<KeepAliveJob>()
    .Build();
trigger = TriggerBuilder.Create()
    .StartNow().WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(5))
    .Build();
await scheduler.ScheduleJob(job, trigger);
job = JobBuilder.Create<BackupJob>()
    .Build();
trigger = TriggerBuilder.Create()
    .StartNow().WithDailyTimeIntervalSchedule(x => x.WithIntervalInMinutes(20))
    .Build();
await scheduler.ScheduleJob(job, trigger);
job = JobBuilder.Create<MYASiteJob>()
    .Build();
trigger = TriggerBuilder.Create()
    .StartNow().WithDailyTimeIntervalSchedule(x => x.WithIntervalInHours(1))
    .Build();
await scheduler.ScheduleJob(job, trigger);
job = JobBuilder.Create<OfflineRewards>()
    .Build();
trigger = TriggerBuilder.Create()
    .StartNow().WithDailyTimeIntervalSchedule(x => x.WithIntervalInHours(1))
    .Build();
await scheduler.ScheduleJob(job, trigger);
job = JobBuilder.Create<MarriedEventService>()
    .Build();
trigger = TriggerBuilder.Create()
    .StartNow().WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(0,0))
    .Build();
await scheduler.ScheduleJob(job, trigger);
job = JobBuilder.Create<AntiSpamJob>()
    .Build();
trigger = TriggerBuilder.Create()
    .StartNow().WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(1))
    .Build();
await scheduler.ScheduleJob(job, trigger);
//==============================================================================//
//Register DI//
ContainerBuilder builder = new();
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
string token = Data.Instance.Container.Resolve<IConfiguration>().Token;
//==============================================================================//
//Start bot//
await _client.LoginAsync(TokenType.Bot, token);
await scheduler.Start();
await _client.StartAsync();

await Task.Delay(-1);

static Task Log(LogMessage msg)
{
    Console.WriteLine(msg.ToString());
    return Task.CompletedTask;
}

internal class ConsoleLogProvider : ILogProvider
{
    public Logger GetLogger(string name)
    {
        return (level, func, exception, parameters) =>
        {
            if (level >= LogLevel.Info && func != null)
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " " + level + " " + func(), parameters);
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