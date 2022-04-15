using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Ben : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("路上你同米亞見到有個變形金剛飛左落來，米亞大叫左一聲高達後個變形金剛似乎極度唔開心就咁飛走左，你完全唔知道要點同米亞解釋...", ephemeral: true);
        }
    }
}
