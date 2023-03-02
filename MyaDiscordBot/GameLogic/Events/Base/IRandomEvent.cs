using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events.Base
{
    public interface IRandomEvent
    {
        Task Response(SocketSlashCommand command, Player player);
    }
}
