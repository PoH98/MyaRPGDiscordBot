using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Luna : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.CurrentHP -= 10;
            if (player.CurrentHP < 1)
            {
                player.CurrentHP = 1;
            }
            return command.RespondAsync("你遇到月之使者露娜，佢邀請你上月球，你決定掉低米亞遠走高飛。結果你太重了，月兔托住你飛到上大氣層嗰陣，你就跌翻落地面，咁啱就跌咗喺基地張床上...", ephemeral: true);
        }
    }
}
