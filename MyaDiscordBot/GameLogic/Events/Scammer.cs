using Discord.WebSocket;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Scammer : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你同米亞路上見到一個古怪男人，佢見到你後就跑過來問你What is your Steam name? 突然小貓出現係你身邊使用左超級一腳踢就踢飛左個古怪男人到空中幾十萬公里以外後消失左！你覺得一頭霧水咩係Steam...", ephemeral: true);
        }
    }
}
