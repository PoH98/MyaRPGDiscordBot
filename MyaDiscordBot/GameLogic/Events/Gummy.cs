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
            player.CurrentHP = player.HP;
            return command.RespondAsync("偶遇類似甘米的女神經過, 瞬間覺得自己身體恢復嗮, 精神奕奕！", ephemeral: true);
        }
    }
}
