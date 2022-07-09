using Autofac;
using Discord.WebSocket;
using Quartz;

namespace MyaDiscordBot.GameLogic.Services
{
    public class OfflineRewardJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = Data.Instance.Container.BeginLifetimeScope())
            {
                var playerService = scope.Resolve<IPlayerService>();
                var client = scope.Resolve<DiscordSocketClient>();
                foreach (var file in Directory.GetFiles("save", "*.db"))
                {
                    //get joined servers
                    try
                    {
                        var guild = client.GetGuild(Convert.ToUInt64(file.Remove(0, file.LastIndexOf("\\") + 1).Replace(".db", "")));
                        var players = playerService.GetPlayers(guild.Id);
                        foreach(var player in players)
                        {
                            //get last command time
                            if(player.LastCommand == DateTime.MinValue)
                            {
                                player.LastCommand = DateTime.Now;
                            }
                            var awaitTime = (DateTime.Now - player.LastCommand).Hours;
                            if(awaitTime < 1)
                            {
                                continue;
                            }
                            var coinGet = awaitTime * 10 * (player.Lv / 10);
                            Random random = new Random();
                            var coinLost = (int)Math.Round((random.NextDouble() * ((awaitTime * 3.5) - (awaitTime * 1.5))) + (awaitTime * 1.5));
                            coinGet -= coinLost;
                            player.Coin += coinGet;
                            player.Exp += coinGet;
                            player.LastCommand = DateTime.Now;
                            playerService.SavePlayer(player);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                }
            }
            return Task.CompletedTask;
        }
    }
}
