using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Smoke : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour is < 6 and > 0)
            {
                return command.RespondAsync("你在路上遇見一名一次性食緊346支煙的熊貓，睇上來就唔好惹，好快速咁離開左依個地方。", ephemeral: true);
            }
            return command.RespondAsync("你在路上遇見一名一次性食緊346支煙的熊貓，米亞見到之後衝上去一大桶水澆滅嗮個熊貓的煙，熊貓非常生氣但好彩你同米亞都跑得快！", ephemeral: true);

        }
    }
}
