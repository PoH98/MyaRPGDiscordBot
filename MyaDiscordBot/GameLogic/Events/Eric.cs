using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Eric : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你在路上突然遇見一個奇怪的人，佢似乎正在搞緊自己隻手唔知道佢想點。你見到佢隔離有個排寫著搞手Eric，覺得奇奇怪怪就拉著米亞離開左。", ephemeral: true);
        }
    }
}
