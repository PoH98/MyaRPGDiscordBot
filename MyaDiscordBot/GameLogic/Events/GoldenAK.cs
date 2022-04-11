﻿using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Events
{
    public class GoldenAK : IRandomEvent
    {
        public Task Response(SocketSlashCommand command, Player player, bool isWall)
        {
            var builder = new ComponentBuilder();
            builder.WithButton("黃金AK", "goldenAK1", ButtonStyle.Success);
            builder.WithButton("鑽石AK", "goldenAK2", ButtonStyle.Success);
            return command.RespondAsync("突然一個熊貓神出現係你面前，手上有一把黃金AK同一把鑽石AK，問你想要邊把！", components: builder.Build() , ephemeral: true);
        }
    }
}
