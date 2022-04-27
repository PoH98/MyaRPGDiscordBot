using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.ButtonEvent
{
    public class SellTypeHandler : IButtonHandler
    {
        private readonly IPlayerService playerService;
        public SellTypeHandler(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            return command.StartsWith("sellType-");
        }

        public Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var cb = new ComponentBuilder();
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            IEnumerable<ItemEquip> items;
            switch (message.Data.CustomId.Replace("sellType-", ""))
            {
                case "fire":
                    items = player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Fire);
                    break;
                case "water":
                    items = player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Water);
                    break;
                case "wind":
                    items = player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Wind);
                    break;
                case "earth":
                    items = player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Earth);
                    break;
                case "light":
                    items = player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Light);
                    break;
                case "dark":
                    items = player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Dark);
                    break;
                default:
                    items = player.Bag.Where(x => x.Type == ItemType.道具);
                    break;
            }
            foreach (var i in items.Where(x => !x.IsEquiped && x.Id != Guid.Empty))
            {
                cb.WithButton(i.Name, "sell-" + i.Id.ToString());
            }
            return message.RespondAsync("你打開左背包發現有...", components: cb.Build(), ephemeral: true);
        }
    }
}
