using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Uncle : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你入左個村落休息，突然有個年輕人來搵你，佢自稱Uncle又係村落的村長，想招呼你個大名鼎鼎的冒險者同米亞，但當你感謝佢唔小心叫左佢Uncle後佢又突然發火趕左你哋出村莊！", ephemeral: true);
        }
    }
}
