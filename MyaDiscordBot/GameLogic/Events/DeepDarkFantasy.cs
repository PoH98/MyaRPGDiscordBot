using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    internal class DeepDarkFantasy : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            Emote emote = Emote.Parse("<:Myabe:927886435829301299>");
            return command.RespondAsync("你在路上突然見到一隻奇異怪獸，佢頭上寫著黑池獸，似乎係一個好強大的敵人。但當你想攻擊佢的時候，佢竟然發出左" + emote.ToString() + "的聲音？！你決定無視個怪獸直接離開！", ephemeral: true);
        }
    }
}
