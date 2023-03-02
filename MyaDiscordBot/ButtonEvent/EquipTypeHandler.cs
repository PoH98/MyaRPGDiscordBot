using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
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
            return command.StartsWith("equipType-");
        }

        public Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            string component = message.Data.CustomId.Replace("equipType-", "");
            Player player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            ComponentBuilder builder = new();
            IEnumerable<ItemEquip> items = component switch
            {
                "fire" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Fire),
                "water" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Water),
                "wind" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Wind),
                "earth" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Earth),
                "light" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Light),
                "dark" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Dark),
                "god" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.God),
                _ => player.Bag.Where(x => x.Type == ItemType.道具),
            };
            foreach (ItemEquip i in items.Where(x => x.IsEquiped))
            {
                _ = builder.WithButton("(" + i.Element + ")" + i.Name, "unequip-" + i.Name.ToLower(), ButtonStyle.Danger);
            }
            foreach (ItemEquip i in items.Where(x => !x.IsEquiped))
            {
                _ = builder.WithButton("(" + i.Element + ")" + i.Name, "equip-" + i.Name.ToLower(), ButtonStyle.Success);
            }
            return message.RespondAsync("你背包內相關的道具：", components: builder.Build(), ephemeral: true);
        }
    }
}
