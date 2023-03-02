using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class SSSMya : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour is < 6 and > 0)
            {
                player.CurrentHP = 1;
                return command.RespondAsync("你在路上見到一塊屎，佢突然開口同你講野，嚇到你唔小心將訓著的米亞dum左落街自己走佬左！米亞被dum醒後發動米亞之壓魔法瞬間傳送到你頭上你被壓扁！", ephemeral: true);
            }
            return command.RespondAsync("你在路上見到一塊識講野的屎，米亞見到後馬上就食左佢！可憐的屎！", ephemeral: true);
        }
    }
}
