using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Sunnyday : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("陽光普照的一日，你同米亞懶洋洋的訓係個樹下，果然係一個好日子！", ephemeral: true);
        }
    }
}
