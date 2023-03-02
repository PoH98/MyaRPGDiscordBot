using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class No : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.NextCommand = DateTime.Now.AddMinutes(10);
            return command.RespondAsync("你路上見到一個大大的路牌寫著“不可以”，你唔知道咩意思正當打算行過個路牌的時候，你發現自己突然無法移動，並且有個彈窗彈出係你面前寫著網絡已斷開連線....", ephemeral: true);
        }
    }
}
