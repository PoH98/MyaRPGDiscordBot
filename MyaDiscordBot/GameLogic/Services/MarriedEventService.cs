using Autofac;
using Discord.WebSocket;
using MyaDiscordBot.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Services
{
    public class MarriedEventService : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope();
            DiscordSocketClient client = scope.Resolve<DiscordSocketClient>();
            IPlayerService playerService = scope.Resolve<IPlayerService>();
            ISettingService settingService = scope.Resolve<ISettingService>();
            List<int> annually = new List<int> { 7, 30, 90, 183, 365, 730, 1095, 1461, 1826, 2191 };
            foreach (string file in Directory.GetFiles("save", "*.db"))
            {
                //get joined servers
                StringBuilder sb = new StringBuilder();
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
                            if(player.MarriedUser > 0)
                            {
                                var days = (int)Math.Round((DateTime.Now - player.MarriedTime).TotalDays);
                                if (annually.Contains(days))
                                {
                                    var p1 = await client.GetUserAsync(player.DiscordId);
                                    var p2 = await client.GetUserAsync(player.MarriedUser);
                                    sb.AppendLine("今日是" + p1.Mention + "與" + p2.Mention + "結婚後的第" + days + "日紀念日，恭喜嗮！");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    ServerSettings setting = settingService.GetSettings(guild.Id);
                    if (sb.Length > 500)
                    {
                        var fullText = sb.ToString().Split("\n");
                        StringBuilder asb = new StringBuilder();
                        for (var index = 0; index < fullText.Length;)
                        {
                            while (asb.Length < 500)
                            {
                                asb.AppendLine(fullText[index]);
                                index++;
                            }
                            await guild.GetTextChannel(setting.ChannelId).SendMessageAsync(asb.ToString());
                            asb.Clear();
                        }
                    }
                    else
                    {
                        await guild.GetTextChannel(setting.ChannelId).SendMessageAsync(sb.ToString());
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
