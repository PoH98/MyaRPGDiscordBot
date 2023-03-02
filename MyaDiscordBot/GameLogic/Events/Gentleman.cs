using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Gentleman : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.NextCommand = DateTime.Now.AddMinutes(20);
            return DateTime.Now.Hour is < 6 and > 0
                ? command.RespondAsync("你見到周身都係屎的紳士，佢眼似乎發出紅光見到你就馬上瞬移到你身邊高速將屎塞落你口入面！你嘔到虛脫!", ephemeral: true)
                : command.RespondAsync("你在路上見到一個紳士似乎係到玩屎，你決定迴避依個周身都係屎的奇怪人，但可惜米亞諗都唔諗直接衝左過去一起玩屎？！你被逼等米亞玩完先可以繼續冒險！", ephemeral: true);
        }
    }
}
