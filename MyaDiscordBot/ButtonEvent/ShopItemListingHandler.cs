using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.GameLogic.Services;
using System.Text;

namespace MyaDiscordBot.ButtonEvent
{
    public class ShopItemListingHandler : IButtonHandler
    {
        private readonly IItemService itemService;
        private readonly IPlayerService playerService;
        public ShopItemListingHandler(IItemService itemService, IPlayerService playerService)
        {
            this.itemService = itemService;
            this.playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            return command.StartsWith("shopType-");
        }

        public Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            Models.Player player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            string[] parts = message.Data.CustomId.Split("-").Skip(1).ToArray();
            IEnumerable<Models.Item> items = itemService.GetShopItem(player);
            //element
            items = parts[0] switch
            {
                "fire" => items.Where(x => x.Element == Models.Element.Fire),
                "water" => items.Where(x => x.Element == Models.Element.Water),
                "wind" => items.Where(x => x.Element == Models.Element.Wind),
                "earth" => items.Where(x => x.Element == Models.Element.Earth),
                "dark" => items.Where(x => x.Element == Models.Element.Dark),
                "light" => items.Where(x => x.Element == Models.Element.Light),
                _ => items.Where(x => x.Element == Models.Element.God),
            };
            items = parts[1] switch
            {
                "weapon" => items.Where(x => x.Type == Models.ItemType.武器),
                "amor" => items.Where(x => x.Type == Models.ItemType.護甲),
                "ring" => items.Where(x => x.Type == Models.ItemType.指環),
                "necklece" => items.Where(x => x.Type == Models.ItemType.頸鏈),
                "shoes" => items.Where(x => x.Type == Models.ItemType.鞋),
                "shield" => items.Where(x => x.Type == Models.ItemType.盾),
                _ => items.Where(x => x.Type == Models.ItemType.道具),
            };
            if (items.Count() < 1)
            {
                return message.RespondAsync("Ermmm小貓搵唔到你想要的野哦！QAQ", ephemeral: true);
            }
            ComponentBuilder builder = new();
            foreach (Models.Item i in items)
            {
                StringBuilder sb = new();
                _ = sb.Append(i.Name);
                if (i.Type == Models.ItemType.道具)
                {
                    _ = sb.Append(" * " + i.UseTimes + " 恢復" + i.HP + "血量");
                }
                else
                {
                    if (i.Atk > 0)
                    {
                        _ = sb.Append(" 提升" + i.Atk + "傷害");
                    }
                    if (i.Def > 0)
                    {
                        _ = sb.Append(" 提升" + i.Def + "護甲");
                    }
                    if (i.HP > 0)
                    {
                        _ = sb.Append(" 提升" + i.HP + "血量");
                    }
                }

                if (i.Ability != Models.Ability.None)
                {
                    switch (i.Ability)
                    {

                    }
                }
                _ = builder.WithButton(sb.ToString() + " - " + i.Price + "$ ", "shop-" + i.Id.ToString());
            }
            return message.RespondAsync("咁以下就係你想要的商品選單啦~喵！≧◉ᴥ◉≦全部都係精心選返來的貨物！", components: builder.Build(), ephemeral: true);
        }
    }
}
