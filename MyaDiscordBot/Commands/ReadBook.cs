using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.Commands
{
    internal class ReadBook : ICommand
    {
        private readonly IPlayerService _playerService;
        public ReadBook(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        public string Name => "read";

        public string Description => "Read Books";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            player.Name = (command.User as SocketGuildUser).DisplayName;
            var cb = new ComponentBuilder();
            if (player.Books.Where(i => i.Amount >= 10).Count() > 0)
            {
                foreach (var i in player.Books.Where(i => i.Amount >= 10))
                {
                    cb.WithButton(i.Name, "read-" + (int)i.BType);
                }
                await command.RespondAsync("你想讀咩書？", components: cb.Build(), ephemeral: true);
            }
            else
            {
                await command.RespondAsync("你冇足夠的書碎片合成一本書！", ephemeral: true);
            }
        }
    }
}
