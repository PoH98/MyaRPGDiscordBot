using Discord.WebSocket;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.GameLogic.Events
{
    public class ALT_F4 : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.NextCommand = DateTime.Now.AddMinutes(10);
            return command.RespondAsync("正當你想伸懶腰的時候突然感覺自己好似唔小心觸發左咩魔法，成功發動ALT+F4..........\n頂米亞大冒險已退出遊戲", ephemeral: true);
        }
    }
}
