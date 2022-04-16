using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Spector : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你發現有個可疑人物匿埋係草叢，正當你衝上前抓著佢之後佢咩都無講就直接係你面前消失左，你聽聞過類似事跡似乎大家都叫個神秘人士察言觀色的昌仁...", ephemeral: true);
        }
    }
}
