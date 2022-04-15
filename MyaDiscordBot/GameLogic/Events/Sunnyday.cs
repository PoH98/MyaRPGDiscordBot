using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Sunnyday : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.CurrentHP += 10;
            if (player.CurrentHP > player.HP)
            {
                player.CurrentHP = player.HP;
            }
            player.NextCommand = DateTime.Now.AddMinutes(30);
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                return command.RespondAsync("已經入夜，你同米亞懶洋洋的訓係個樹下，涼風吹過又安寧，果然係一個好日子！", ephemeral: true);
            }
            return command.RespondAsync("陽光普照的一日，你同米亞懶洋洋的訓係個樹下，果然係一個好日子！", ephemeral: true);
        }
    }
}
