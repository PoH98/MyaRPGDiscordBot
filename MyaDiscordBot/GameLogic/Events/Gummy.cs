using Discord.WebSocket;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Gummy : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            throw new NotImplementedException();
        }
    }
}
