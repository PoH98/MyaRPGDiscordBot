using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Daodi : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour is < 6 and > 0)
            {
                player.CurrentHP = 1;
                return command.RespondAsync("米亞訓著的時候講夢話，似乎係咩道地Game System Protection系統啟動，你唔明白係咩意思，但突然見到一個導彈從天而降，你被炸成殘血！", ephemeral: true);
            }
            return command.RespondAsync("你同米亞在遠處見到一個超高科技的城鎮，米亞興奮的跑在前面，你追都追唔上，突然有個巡邏空中戰艦停左係你同米亞的頭上，上面寫著道地號MKV-Game System Protection AI，佢警告你哋不能接近該區域而且嘗試開炮，你同米亞嚇得馬上離開左個地方", ephemeral: true);
        }
    }
}
