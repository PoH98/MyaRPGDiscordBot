﻿using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;

namespace MyaDiscordBot.Commands
{
    public class Dice : ICommand
    {
        public string Name => "dice";

        public string Description => "throw a dice";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            Random random = new();
            return command.RespondAsync("獲得" + random.Next(1, 7));
        }
    }
}
