using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class CheukCat : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你同米亞來到一個城鎮，睇到一隻可愛的貓耳女仔正在畫畫，而且畫得非常靚！貓耳女仔自稱係旅行畫家，然後就幫米亞畫左一副畫後消失在茫茫人海中，米亞開心到抱著個畫一隻唔肯鬆開", ephemeral: true);
        }
    }
}
