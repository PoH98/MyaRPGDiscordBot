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
            using (ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope())
            {
                DiscordSocketClient client = scope.Resolve<DiscordSocketClient>();
                if (client.ConnectionState is Discord.ConnectionState.Disconnected or Discord.ConnectionState.Disconnecting)
                {
                    _ = Process.Start("MyaDiscordBot.exe");
                    Environment.Exit(0);
                }
            }
            return Task.CompletedTask;
        }
    }
}
