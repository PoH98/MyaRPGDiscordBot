using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Salmon : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                player.CurrentHP += 10;
                if (player.CurrentHP > player.HP)
                {
                    player.CurrentHP = player.HP;
                }
                player.NextCommand = DateTime.Now.AddMinutes(30);
                return command.RespondAsync("你路過一個三文魚店，不過似乎已經關門，你只好搵附近的酒店休息下先！", ephemeral: true);
            }
            return command.RespondAsync("你在路上見到在賣三文魚的三文魚，佢突然叫著你話你骨骼驚奇，需要大量三文魚補充身體，送左倆條三文魚卑你，可惜米亞手速太快直接生吞左全部送你的三文魚！", ephemeral: true);
        }
    }
}
