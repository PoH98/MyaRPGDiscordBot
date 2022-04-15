using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class MyaNoMoney : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.Coin -= 3;
            return command.RespondAsync("米亞走開左去遠處的廁所買太多屎食，最後無錢搭車返你身邊，你無奈被迫快轉左3蚊卑佢", ephemeral: true);
        }
    }
}
