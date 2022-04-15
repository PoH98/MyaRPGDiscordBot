using Discord.WebSocket;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Yoyo : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("路上你睇到個搖搖好強的戰士用搖搖快速攻擊多個敵人，你同米亞坐底係個樹下野餐睇佢戰鬥直到佢走左為止", ephemeral: true);
        }
    }
}
