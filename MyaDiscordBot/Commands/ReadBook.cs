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
        private readonly IBattleService _battleService;
        private readonly IEventService _eventService;
        private readonly IBossService _bossService;
        private readonly IItemService _itemService;
        public ReadBook(IPlayerService playerService, IBattleService battleService, IBossService bossService, IEventService eventService, IItemService itemService)
        {
            _playerService = playerService;
            _battleService = battleService;
            _eventService = eventService;
            _bossService = bossService;
            _itemService = itemService;
        }
        public string Name => "read";

        public string Description => "Read Books";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            player.Name = (command.User as SocketGuildUser).DisplayName;
            var cb = new ComponentBuilder();
            if (player.Books.Where(i => i.Value >= 10).Count() > 0)
            {
                foreach (var i in player.Books.Where(i => i.Value >= 10))
                {
                    cb.WithButton(i.Key.Name, "read-" + i.Key);
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
