using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
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
            player.NextCommand = DateTime.Now.AddMinutes(20);
            return DateTime.Now.Hour is < 6 and > 0
                ? command.RespondAsync("已經入夜，你同米亞懶洋洋的訓係個樹下，涼風吹過又安寧，果然係一個好日子！", ephemeral: true)
                : command.RespondAsync("陽光普照的一日，你同米亞懶洋洋的訓係個樹下，果然係一個好日子！", ephemeral: true);
        }
    }
}
