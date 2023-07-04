using Autofac;
using Discord.WebSocket;
using MyaDiscordBot.Models;
using Quartz;
using System.Net.NetworkInformation;

namespace MyaDiscordBot.GameLogic.Services
{
    internal class OfflineRewards : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope();
            DiscordSocketClient client = scope.Resolve<DiscordSocketClient>();
            IPlayerService playerService = scope.Resolve<IPlayerService>();
            foreach (string file in Directory.GetFiles("save", "*.db"))
            {
                //get joined servers
                try
                {
                    if (!NetworkInterface.GetIsNetworkAvailable())
                    {
                        return;
                    }
                    SocketGuild guild = client.GetGuild(Convert.ToUInt64(file.Remove(0, file.LastIndexOf("\\") + 1).Replace(".db", "")));
                    if (guild == null)
                    {
                        //guild is not exist anymore, bot is kicked
                        var f = new FileInfo(file);
                        f.MoveTo(file + ".removed");
                        continue;
                    }
                    List<Player> players = playerService.GetPlayers(guild.Id);
                    foreach (Player player in players)
                    {
                        try
                        {
                            //get last command time
                            if (player.LastCommand == DateTime.MinValue)
                            {
                                player.LastCommand = DateTime.Now;
                            }
                            int awaitTime = (DateTime.Now - player.LastCommand).Hours;
                            if (awaitTime < 1)
                            {
                                continue;
                            }
                            int coinGet = awaitTime * 10 * (player.Lv / 10);
                            Random random = new();
                            int coinLost = (int)Math.Round((random.NextDouble() * ((awaitTime * 3.5) - (awaitTime * 1.5))) + (awaitTime * 1.5));
                            coinGet -= coinLost;
                            if (coinGet <= 0)
                            {
                                coinGet = 1;
                            }
                            player.Coin += coinGet;
                            player.Exp += (int)Math.Round(coinGet * 0.2);
                            player.LastCommand = DateTime.Now;
                            if (player.Exp >= 100000)
                            {
                                player.Exp = 100000;
                            }
                            playerService.SavePlayer(player);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }
    }
}
