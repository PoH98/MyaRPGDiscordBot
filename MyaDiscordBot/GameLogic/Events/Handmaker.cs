using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Handmaker : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("經過一連串的路程，你決定要休息下，不過突然見到個衫上寫著Handmaker的人係路旁似乎整緊個告示，等你行過去睇先發現竟然係有關米亞的萌娘百科？！\nhttps://zh.moegirl.org.cn/index.php?title=%E7%B1%B3%E4%BA%9AMYA", ephemeral: true);
        }
    }
}
