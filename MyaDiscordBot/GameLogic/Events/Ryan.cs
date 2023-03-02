using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Ryan : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("路上你見到個人上面有個彩虹字寫著\"最大測試員Ryan\"路過，似乎佢經過的地方都會出現不可思議的裂縫或者破損？！", ephemeral: true);
        }
    }
}
