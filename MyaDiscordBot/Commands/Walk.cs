using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Commands
{
    public class Walk : ICommand
    {
        private IPlayerService _playerService;
        public Walk(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        public string Name => "walk";

        public string Description => "Walk around at this floor";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1]
        {
            new SlashCommandOptionBuilder().WithName("direction").WithDescription("Walk Direction").WithRequired(true).WithType(ApplicationCommandOptionType.Integer).AddChoice("Front", 1).AddChoice("Back", 2).AddChoice("Left", 3).AddChoice("Right", 4)
        };

        public async Task Handler(SocketSlashCommand command)
        {
            var player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            
            await command.RespondAsync("March to " + command.Data.Options.First().Value);
        }
    }
}
