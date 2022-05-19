using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.ButtonEvent
{
    public class EquipTypeHandler : IButtonHandler
    {
        private readonly IPlayerService playerService;

        public EquipTypeHandler(IPlayerService playerService)
        {
            this.playerService = playerService;
        }


        public bool CheckUsage(string command)
        {
            if (command.StartsWith("equipType-"))
            {
                return true;
            }
            return false;
        }

        public Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var component = message.Data.CustomId.Replace("equipType-", "");
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            var builder = new ComponentBuilder();
            IEnumerable<ItemEquip> items;
            switch (component)
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
                case "god":
                    items = player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.God);
                    break;
                default:
                    items = player.Bag.Where(x => x.Type == ItemType.道具);
                    break;
            }
            foreach (var i in items.Where(x => x.IsEquiped))
            {
                builder.WithButton("(" + i.Element + ")" + i.Name, "unequip-" + i.Name.ToLower(), ButtonStyle.Danger);
            }
            foreach (var i in items.Where(x => !x.IsEquiped))
            {
                builder.WithButton("(" + i.Element + ")" + i.Name, "equip-" + i.Name.ToLower(), ButtonStyle.Success);
            }
            return message.RespondAsync("你背包內相關的道具：", components: builder.Build(), ephemeral: true);
        }
    }
}
