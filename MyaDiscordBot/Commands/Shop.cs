﻿using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Shop : ICommand
    {
        private IPlayerService playerService;
        public Shop(IPlayerService playerService) 
        {
            this.playerService = playerService;
        }
        public string Name => "shop";

        public string Description => "Shop purchase";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            ComponentBuilder builder = new();
            _ = builder.WithButton("光屬性裝備", "shopEle-light").WithButton("暗屬性裝備", "shopEle-dark").WithButton("風屬性裝備", "shopEle-wind").WithButton("火屬性裝備", "shopEle-fire").WithButton("地屬性裝備", "shopEle-earth").WithButton("水屬性裝備", "shopEle-water").WithButton("道具/消耗品", "shopType-none-none");
            if (playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id).Lv >= 90)
            {
                _ = builder.WithButton("神屬性裝備", "shopEle-god");
            }
            await command.RespondAsync("你搖一搖院友卡，小貓精靈就馬上受到你的召喚，已經出現係你面前！≧◉ᴥ◉≦", components: builder.Build(), ephemeral: true);
        }
    }
}
