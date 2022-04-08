using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public interface IRandomEvent
    {
        Task Response(SocketSlashCommand command, Player player);
    }
}
