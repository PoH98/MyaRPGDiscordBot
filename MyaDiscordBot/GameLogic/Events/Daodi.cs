using Discord.WebSocket;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Daodi : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你同米亞在遠處見到一個超高科技的城鎮，米亞興奮的跑在前面，你追都追唔上，突然有個巡邏空中戰艦停左係你同米亞的頭上，上面寫著道地號MKV-Game System Protection AI，佢警告你哋不能接近該區域而且嘗試開炮，你同米亞嚇得馬上離開左個地方", ephemeral: true);
        }
    }
}
