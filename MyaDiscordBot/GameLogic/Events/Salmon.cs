using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Salmon : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你在路上見到在賣三文魚的三文魚，佢突然叫著你話你骨骼驚奇，需要大量三文魚補充身體，送左倆條三文魚卑你，可惜米亞手速太快直接生吞左全部送你的三文魚！", ephemeral: true);
        }
    }
}
