﻿using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models.Books;

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
            string[] parts = message.Data.CustomId.Split('-');
            Models.Player player = _playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            Book book = new()
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
                if (player.Atk >= 9999)
                {
                    player.Atk = 9999;
                    _playerService.SavePlayer(player);
                    await message.RespondAsync("你睇完本書後，發現已經無法再增強自己！無敵的感覺真係爽快！", ephemeral: true);
                    return;
                }
                if (player.Def >= 999)
                {
                    player.Def = 999;
                    _playerService.SavePlayer(player);
                    await message.RespondAsync("你睇完本書後，發現已經無法再增強自己！無敵的感覺真係爽快！", ephemeral: true);
                    return;
                }
                if (player.HP >= 9999)
                {
                    player.HP = 9999;
                    _playerService.SavePlayer(player);
                    await message.RespondAsync("你睇完本書後，發現已經無法再增強自己！無敵的感覺真係爽快！", ephemeral: true);
                    return;
                }
                _playerService.SavePlayer(player);
                await message.RespondAsync("你睇完本書後，覺得自己變強啦！", ephemeral: true);
                return;
            }
            await message.RespondAsync("你的書碎片唔夠！無法合成到任何一本完整的書！", ephemeral: true);

        }
    }
}
