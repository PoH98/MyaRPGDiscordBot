using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Tissue : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                return command.RespondAsync("你背著訓著的米亞的時候突然有一大疊紙巾從米亞的背包跌左，似乎紙巾都已經用過？！你覺得好嘔心馬上快步離開唔想見到疊紙巾！", ephemeral: true);
            }
            return command.RespondAsync("米亞突然拎出來一堆紙巾，話係男人的象征，好明顯有全部紙巾都用過嗮？！你覺得好嘔心命令米亞dum左佢！", ephemeral: true);
        }
    }
}
