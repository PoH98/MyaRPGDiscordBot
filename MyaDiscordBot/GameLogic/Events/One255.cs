using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class One255 : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你見到個叫1255的店，決定入去睇下係咩店，發現原來係全部商品都賣1255蚊！嚇到你衝出店門！", ephemeral: true);
        }
    }
}
