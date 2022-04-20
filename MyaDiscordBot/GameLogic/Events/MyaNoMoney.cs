using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class MyaNoMoney : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                player.Coin -= 6;
                player.NextCommand = DateTime.Now.AddMinutes(20);
                return command.RespondAsync("你路上背著個訓著的米亞，睇到個男人半夜係到收衫，佢同你講左一句落雨後就突然暴風雨來左，你只好去佢個屋企暫住，而且被逼俾左佢6蚊住宿費...", ephemeral: true);
            }
            player.Coin -= 3;
            return command.RespondAsync("米亞走開左去遠處的廁所買太多屎食，最後無錢搭車返你身邊，你無奈被迫快轉左3蚊卑佢", ephemeral: true);
        }
    }
}
