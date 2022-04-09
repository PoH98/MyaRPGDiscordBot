using Discord.WebSocket;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Tissue : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("米亞突然拎出來一堆紙巾，話係男人的象征，好明顯有全部紙巾都用過嗮？！你覺得好嘔心命令米亞dum左佢！");
        }
    }
}
