using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class SaintNoName : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你在尋人啟事上面見到一個特殊的公告，似乎尋找緊一個無名的聖者。你決定要接下任務，不過可惜突然來左一個白髮的冒險家，睇都無睇就將整個版面所有任務接走嗮離開左！", ephemeral: true);
        }
    }
}
