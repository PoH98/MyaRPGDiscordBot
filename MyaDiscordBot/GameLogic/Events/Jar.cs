using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Jar : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你在路上見到個唔知道點解卡係個壺入面的男人係到爬山，然後不斷咁跌落來。睇得你都想打人！", ephemeral: true);
        }
    }
}
