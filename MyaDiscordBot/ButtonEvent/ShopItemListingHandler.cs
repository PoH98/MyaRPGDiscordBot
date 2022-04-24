using Discord;
using Discord.WebSocket;
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
            if (command.StartsWith("shopType-"))
            {
                return true;
            }
            return false;
        }

        public Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            var parts = message.Data.CustomId.Split("-").Skip(1).ToArray();
            var items = itemService.GetShopItem(player);
            //element
            switch (parts[0])
            {
                case "fire":
                    items = items.Where(x => x.Element == Models.Element.Fire);
                    break;
                case "water":
                    items = items.Where(x => x.Element == Models.Element.Water);
                    break;
                case "wind":
                    items = items.Where(x => x.Element == Models.Element.Wind);
                    break;
                case "earth":
                    items = items.Where(x => x.Element == Models.Element.Earth);
                    break;
                case "dark":
                    items = items.Where(x => x.Element == Models.Element.Dark);
                    break;
                case "light":
                    items = items.Where(x => x.Element == Models.Element.Light);
                    break;
                default:
                    items = items.Where(x => x.Element == Models.Element.God);
                    break;
            }
            switch (parts[1])
            {
                case "weapon":
                    items = items.Where(x => x.Type == Models.ItemType.武器);
                    break;
                case "amor":
                    items = items.Where(x => x.Type == Models.ItemType.護甲);
                    break;
                case "ring":
                    items = items.Where(x => x.Type == Models.ItemType.指環);
                    break;
                case "necklece":
                    items = items.Where(x => x.Type == Models.ItemType.頸鏈);
                    break;
                case "shoes":
                    items = items.Where(x => x.Type == Models.ItemType.鞋);
                    break;
                case "shield":
                    items = items.Where(x => x.Type == Models.ItemType.盾);
                    break;
                default:
                    items = items.Where(x => x.Type == Models.ItemType.道具);
                    break;
            }
            if (items.Count() < 1)
            {
                return message.RespondAsync("Ermmm小貓搵唔到你想要的野哦！QAQ", ephemeral: true);
            }
            var builder = new ComponentBuilder();
            foreach (var i in items)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(i.Name);
                if (i.Type == Models.ItemType.道具)
                {
                    sb.Append(" * " + i.UseTimes + " 恢復" + i.HP + "血量");
                }
                else
                {
                    if (i.Atk > 0)
                    {
                        sb.Append(" 提升" + i.Atk + "傷害");
                    }
                    if (i.Def > 0)
                    {
                        sb.Append(" 提升" + i.Def + "護甲");
                    }
                    if (i.HP > 0)
                    {
                        sb.Append(" 提升" + i.HP + "血量");
                    }
                }

                if (i.Ability != Models.Ability.None)
                {
                    switch (i.Ability)
                    {

                    }
                }
                builder.WithButton(sb.ToString() + " - " + i.Price + "$ ", "shop-" + i.Id.ToString());
            }
            return message.RespondAsync("咁以下就係你想要的商品選單啦~喵！≧◉ᴥ◉≦全部都係精心選返來的貨物！", components: builder.Build(), ephemeral: true);
        }
    }
}
