using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class PotatoAgain : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你喺路上見到舊識行路嘅蕃薯，蕃薯跳入草叢後就消失咗...", ephemeral: true);
        }
    }
}
