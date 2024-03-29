﻿using Discord;
using Discord.WebSocket;

namespace MyaDiscordBot.Commands.Base
{
    internal interface ICommand
    {
        string Name { get; }
        string Description { get; }
        Task Handler(SocketSlashCommand command, DiscordSocketClient client);
        IEnumerable<SlashCommandOptionBuilder> Option { get; }
    }
}
