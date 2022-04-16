using Discord.WebSocket;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Bagu : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你行下行下發現前方似乎出現一陣濃霧無法前進，只好準備休息下等待濃霧驅散，米亞突然發現有個充滿血跡的白狐係濃霧裡面。你覺得事情好似唔簡單就衝左入去，當你都睇到個白狐同身邊有個尸體的時候濃霧突然消失，白狐同尸體都隨之唔見左！", ephemeral:true);
        }
    }
}
