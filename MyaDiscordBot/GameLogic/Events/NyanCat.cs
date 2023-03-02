using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class NyanCat : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            Random rnd = new();
            int coin = rnd.Next(5, 10);
            player.Coin += coin;
            return command.RespondAsync("你在路上見到小貓精靈似乎遇見左幾個怪物，你衝上去幫小貓打敗左佢哋！小貓為左表示感激卑左你" + coin + "蚊！", ephemeral: true);
        }
    }
}
