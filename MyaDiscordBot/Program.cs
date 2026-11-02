// See https://aka.ms/new-console-template for more information
using Autofac;
using Discord;
using Discord.WebSocket;
using MyaDiscordBot;
using MyaDiscordBot.Extension;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using Quartz;
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
IScheduler scheduler = await QuartzSchedulerBuilder.Create().BuildScheduler();
if (!File.Exists("disableGame.txt"))
{
    var gamejob = JobBuilder.Create<BossJob>().WithIdentity("Boss Job").Build();
    var gametrigger = TriggerBuilder.Create()
        .StartNow().WithDailyTimeIntervalSchedule(x => x.WithInterval(1, IntervalUnit.Hour))
        .Build();
    await scheduler.ScheduleJob(gamejob, gametrigger);
    gamejob = JobBuilder.Create<MYASiteJob>().WithIdentity("Mya Site Job")
    .Build();
    gametrigger = TriggerBuilder.Create()
        .StartNow().WithDailyTimeIntervalSchedule(x => x.WithInterval(1, IntervalUnit.Hour))
        .Build();
    await scheduler.ScheduleJob(gamejob, gametrigger);
    gamejob = JobBuilder.Create<OfflineRewards>().WithIdentity("Offline Job")
        .Build();
    gametrigger = TriggerBuilder.Create()
        .StartNow().WithDailyTimeIntervalSchedule(x => x.WithInterval(1, IntervalUnit.Hour))
        .Build();
    await scheduler.ScheduleJob(gamejob, gametrigger);
    gamejob = JobBuilder.Create<MarriedEventService>().WithIdentity("Marry Job")
        .Build();
    gametrigger = TriggerBuilder.Create()
        .StartNow().WithSchedule(CronScheduleBuilder.Create("0 0 0 * * ?"))
        .Build();
    await scheduler.ScheduleJob(gamejob, gametrigger);
}
var job = JobBuilder.Create<BackupJob>().WithIdentity("Backup Job")
    .Build();
var trigger = TriggerBuilder.Create()
    .StartNow().WithDailyTimeIntervalSchedule(x => x.WithInterval(20, IntervalUnit.Minute))
    .Build();
await scheduler.ScheduleJob(job, trigger);
job = JobBuilder.Create<AntiSpamJob>().WithIdentity("Anti Spam Job")
    .Build();
trigger = TriggerBuilder.Create()
    .StartNow().WithDailyTimeIntervalSchedule(x => x.WithInterval(1, IntervalUnit.Second))
    .Build();
await scheduler.ScheduleJob(job, trigger);
//==============================================================================//
//Register DI//
Autofac.ContainerBuilder builder = new();
//Load all commands
builder.RegisterInterfaces();
//Register Socket Client
builder.RegisterInstance(_client);
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