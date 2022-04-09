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
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("「咕咕咕~」，突然有一pat雀屎跌咗落米亞個頭度，米亞為發洩完隻生吞咗咕咕鳥Σ( ° △ °|||)︴", ephemeral: true);
        }
    }
}
