using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class Moses : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            return command.RespondAsync("你同米亞來到一個激流，在無辦法度過激流的時候突然出現一個男人，佢講佢可以幫忙後對著激流敲左一下地板，激流就分開左一半出現一個可以通行的道路！你同米亞快速通過後發現男人已經消失，而且激流恢復返原樣。你思考左好耐都唔知道佢係邊個...", ephemeral: true);
        }
    }
}
