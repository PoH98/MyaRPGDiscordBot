using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Commands
{
    public class Shop : ICommand
    {
        public string Name => "shop";

        public string Description => "Shop purchase";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
