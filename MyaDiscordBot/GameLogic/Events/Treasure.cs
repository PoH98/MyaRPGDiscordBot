using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Treasure : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            Random rnd = new();
            int coin = rnd.Next(1, 5);
            player.Coin += coin;
            return command.RespondAsync("你發現有個" + coin + "蚊係馬路上，於是神不知鬼不覺咁執走左！", ephemeral: true);
        }
    }
}
