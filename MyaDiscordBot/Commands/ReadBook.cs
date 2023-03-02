using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;

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
            Models.Player player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            player.Name = (command.User as SocketGuildUser).DisplayName;
            ComponentBuilder cb = new();
            if (player.Books.Where(i => i.Amount >= 10).Count() > 0)
            {
                foreach (Models.Books.Book i in player.Books.Where(i => i.Amount >= 10))
                {
                    _ = cb.WithButton(i.Name, "read-" + (int)i.BType);
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
