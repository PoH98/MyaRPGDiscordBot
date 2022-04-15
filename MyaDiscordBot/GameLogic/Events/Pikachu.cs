using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Pikachu : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("米亞唔知道去左邊，突然一聲慘叫後，有隻皮卡超向你走左過來？！經過一系列讀心術後先知道原來米亞唔小心掂到個不知名魔法方塊變成左皮卡超！好彩無幾耐後米亞終於變返原狀", ephemeral: true);
        }
    }
}
