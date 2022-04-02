using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Commands
{
    public class Search : ICommand
    {
        public string Name => "Search";

        public string Description => "Search around at this floor";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command)
        {
            var id = command.User.AvatarId;
            command.RespondAsync();
        }
    }
}
