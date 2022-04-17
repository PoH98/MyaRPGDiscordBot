using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Treasure : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.Coin++;
            return command.RespondAsync("你發現有個1蚊係馬路上，於是神不知鬼不覺咁執走左！", ephemeral: true);
        }
    }
}
