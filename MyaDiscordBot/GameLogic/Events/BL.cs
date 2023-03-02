using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class BL : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.Coin -= 30;
            return command.RespondAsync("你經過書店的時候，突然見到JC12同阿反的BL本？！雖然你唔知道佢哋係邊個不過突然衝動的買左10本返來！花費30蚊！", ephemeral: true);
        }
    }
}
