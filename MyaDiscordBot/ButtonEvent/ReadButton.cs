using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models.Books;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.ButtonEvent
{
    internal class ReadButton : IButtonHandler
    {
        private readonly IPlayerService _playerService;
        public ReadButton(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            return command.StartsWith("read-");
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var parts = message.Data.CustomId.Split('-');
            var player = _playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            var book = new Book()
            {
                BType = (BookType)Convert.ToInt32(parts[1])
            };
            if (player.Books.Any(y => y.BType == book.BType && y.Amount >= 10))
            {
                player.Books.First(x => x.BType == book.BType).Amount -= 10;
                switch (book.BType)
                {
                    case BookType.Atk:
                        player.Atk++;
                        break;
                    case BookType.Def:
                        player.Def++;
                        break;
                    case BookType.HP:
                        player.HP += 15;
                        break;
                }
                _playerService.SavePlayer(player);
                await message.RespondAsync("你睇完本書後，覺得自己變強啦！", ephemeral: true);
                return;
            }
            await message.RespondAsync("你的書碎片唔夠！無法合成到任何一本完整的書！", ephemeral: true);

        }
    }
}
