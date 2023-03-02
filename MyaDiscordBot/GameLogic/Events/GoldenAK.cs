using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Events.Base;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class GoldenAK : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player)
        {
            ComponentBuilder builder = new();
            _ = builder.WithButton("黃金AK", "goldenAK1", ButtonStyle.Success);
            _ = builder.WithButton("鑽石AK", "goldenAK2", ButtonStyle.Success);
            return command.RespondAsync("突然一個熊貓神出現係你面前，手上有一把黃金AK同一把鑽石AK，問你想要邊把！", components: builder.Build(), ephemeral: true);
        }
    }
}
