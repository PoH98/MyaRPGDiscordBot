using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Pizza : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            player.Coin -= 30;
            player.NextCommand = DateTime.Now.AddMinutes(10);
            return command.RespondAsync("你見到個Pizza店，米亞衝左入去食，你被迫要跟著入間店，冇左30蚊並且需要等米亞食完先！", ephemeral: true);
        }
    }
}
