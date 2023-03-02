using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    internal class Book : ICommand
    {
        private readonly IPlayerService playerService;
        public Book(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "book";

        public string Description => "Check your home library";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            Models.Player player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            player.Name = (command.User as SocketGuildUser).DisplayName;
            EmbedBuilder eb = new() { Color = Color.Orange };
            _ = eb.WithTitle(player.Name);
            _ = eb.WithDescription("你家中的書本碎片：");
            foreach (Models.Books.Book book in player.Books)
            {
                _ = eb.AddField(book.Name, book.Amount);
            }
            return command.RespondAsync(embed: eb.Build(), ephemeral: true);
        }
    }
}
