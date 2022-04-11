using Discord.WebSocket;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Gugu : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player, bool isWall)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                return command.RespondAsync("zzZZZ (米亞已經訓著，所以無任何特別事件哦！)", ephemeral: true);
            }
            if (isWall)
            {
                return command.RespondAsync("你對著不可行走的區域原地踏步後，「咕咕咕~」，突然有一pat雀屎跌咗落米亞個頭度，米亞為發洩完隻生吞咗咕咕鳥Σ( ° △ °|||)︴", ephemeral: true);
            }
            else
            {
                return command.RespondAsync("「咕咕咕~」，突然有一pat雀屎跌咗落米亞個頭度，米亞為發洩完隻生吞咗咕咕鳥Σ( ° △ °|||)︴", ephemeral: true);
            }

        }
    }
}
