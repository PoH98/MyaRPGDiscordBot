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
        public string Name => "heal";

        public string Description => "Add back 10 HP to your current HP but not more than max HP";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
