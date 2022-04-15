using Discord.WebSocket;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Thomas : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你同米亞來到一個叫Thomas之城的國家，每個人似乎都叫Thomas。米亞覺得好有趣就跑左入去搞左個居民證，名叫Thomyas??", ephemeral: true);
        }
    }
}
