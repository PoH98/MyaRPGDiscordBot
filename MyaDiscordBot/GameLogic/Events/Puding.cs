using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Puding : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour is < 6 and > 0)
            {
                return command.RespondAsync("你路過一個賣布甸的店，可惜已經關門，雖然肚餓不過都係算啦！", ephemeral: true);
            }
            player.Coin -= 40;
            return command.RespondAsync("米亞見到個賣布甸的店，就直接拉著你衝左入去，最後米亞買左超多布甸先肯出門口，你無左40$，覺得自己幾乎要破產！", ephemeral: true);
        }
    }
}
