using Autofac;
using Discord.WebSocket;
using Quartz;
using System.Diagnostics;

namespace MyaDiscordBot.GameLogic.Services
{
    public class KeepAliveJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = Data.Instance.Container.BeginLifetimeScope())
            {
                var client = scope.Resolve<DiscordSocketClient>();
                if (client.ConnectionState == Discord.ConnectionState.Disconnected)
                {
                    Process.Start("MyaDiscordBot.exe");
                    Environment.Exit(0);
                }
            }
            return Task.CompletedTask;
        }
    }
}
