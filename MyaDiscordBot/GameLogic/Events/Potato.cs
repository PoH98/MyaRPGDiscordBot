using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Potato : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                return command.RespondAsync("路上你突然見到有一班薯仔過馬路，你等佢哋行走嗮後先繼續旅程，但突然你先發現剛剛好似係整班薯仔會行路？！", ephemeral: true);
            }
            return command.RespondAsync("米亞唔知道邊到搵到一大堆薯仔，正當你哋想煮左佢哋來食的時候，突然發現其中一個薯仔竟然有腳突然走佬左，真莫名其妙！", ephemeral: true);
        }
    }
}
