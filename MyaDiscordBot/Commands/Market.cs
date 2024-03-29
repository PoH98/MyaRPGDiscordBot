﻿using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.Commands
{
    public class Market : ICommand
    {
        private readonly IMarketService marketService;
        private readonly IPlayerService playerService;
        private readonly Resources resources;
        public Market(IMarketService marketService, IPlayerService playerService, Resources resources)
        {
            this.marketService = marketService;
            this.playerService = playerService;
            this.resources = resources;
        }
        public string Name => "market";

        public string Description => "Market place for selling or purchasing resource between players, will tax 6% on sold price";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1]
        {
            new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.Number).AddChoice("Sell", 1).AddChoice("Buy", 2).WithName("method").WithDescription("What you want to do in market?").WithRequired(true)
        };

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            if (command.Data.Options.First().Value.ToString() == "2")
            {
                IEnumerable<MarketData> data = marketService.GetMarketData((command.Channel as SocketGuildChannel).Guild.Id);
                if (data.Count() > 0)
                {
                    SelectMenuBuilder sb = new();
                    _ = sb.WithMinValues(1);
                    _ = sb.WithMaxValues(1);
                    _ = sb.WithCustomId(Guid.NewGuid().ToString());
                    foreach (MarketData d in data.OrderBy(x => x.Price / x.Amount).Take(25))
                    {
                        Resource r = resources.First(x => x.Id.ToString() == d.ResourceId);
                        _ = sb.AddOption(r.Name, "market-" + d.Id.ToString(), "數量：" + d.Amount + " 價錢：" + d.Price);
                    }
                    ComponentBuilder cb = new();
                    _ = cb.WithSelectMenu(sb);
                    return command.RespondAsync("", components: cb.Build(), ephemeral: true);
                }
                else
                {
                    return command.RespondAsync("市集一個人都無，你懷疑自己入左異次元空間！", ephemeral: true);
                }
            }
            else
            {
                Player player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
                player.ResourceBag ??= new List<HoldedResource>();
                if (player.ResourceBag.Count() > 0 && player.ResourceBag.Any(x => x.Amount > 0))
                {
                    ComponentBuilder cb = new();
                    foreach (HoldedResource b in player.ResourceBag.Where(x => x.Amount > 0))
                    {
                        _ = cb.WithButton(b.Name, "marketSell-" + b.Id);
                    }
                    return command.RespondAsync("選擇你想出售的野！", components: cb.Build(), ephemeral: true);
                }
                else
                {
                    return command.RespondAsync("你無野可以賣！", ephemeral: true);
                }
            }
        }
    }
}
