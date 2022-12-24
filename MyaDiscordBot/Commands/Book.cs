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
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            player.Name = (command.User as SocketGuildUser).DisplayName;
            EmbedBuilder eb = new EmbedBuilder() { Color = Color.Orange };
            eb.WithTitle(player.Name);
            eb.WithDescription("你家中的書本碎片：");
            foreach(var book in player.Books)
            {
                eb.AddField(book.Name, book.Amount);
            }
            return command.RespondAsync(embed: eb.Build(), ephemeral: true);
        }
    }
}
