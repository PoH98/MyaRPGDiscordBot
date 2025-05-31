using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Daodi : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour is < 6 and > 0)
            {
                player.CurrentHP = 1;
                return command.RespondAsync("你遇見一個奇怪的裝置，上面寫著Administrator道地Danny，你想好似米亞咁唔知道輸入咩可以獲得金幣，但突然個裝置爆炸！你剩殘血！", ephemeral: true);
            }
            player.Coin += 1;
            return command.RespondAsync("你遇見一個奇怪的裝置，上面寫著Administrator道地Danny，米亞馬上就衝上去亂輸入一通，你獲1金幣！", ephemeral: true);
        }
    }
}
