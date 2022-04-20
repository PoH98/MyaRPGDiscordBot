using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class CheukCat : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                player.NextCommand = DateTime.Now.AddMinutes(20);
                return command.RespondAsync("你路上見到貓耳女仔係到燒烤緊，似乎準備係到過左依個夜晚，佢睇你孭著訓著的米亞就約你一起來休息下，期間佢幫訓著的米亞畫左一副畫後送左卑你！", ephemeral: true);
            }
            return command.RespondAsync("你同米亞來到一個城鎮，睇到一隻可愛的貓耳女仔正在畫畫，而且畫得非常靚！貓耳女仔自稱係旅行畫家，然後就幫米亞畫左一副畫後消失在茫茫人海中，米亞開心到抱著個畫一隻唔肯鬆開", ephemeral: true);
        }
    }
}
