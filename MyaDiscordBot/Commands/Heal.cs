using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Commands
{
    public class Heal : ICommand
    {
        public string Name => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public IEnumerable<SlashCommandOptionBuilder> Option => throw new NotImplementedException();

        public Task Handler(SocketSlashCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
