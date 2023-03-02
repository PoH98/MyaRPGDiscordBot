using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;

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
            ComponentBuilder cb = new();
            Player player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            IEnumerable<ItemEquip> items = message.Data.CustomId.Replace("sellType-", "") switch
            {
                "fire" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Fire),
                "water" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Water),
                "wind" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Wind),
                "earth" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Earth),
                "light" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Light),
                "dark" => player.Bag.Where(x => x.Type != ItemType.道具 && x.Element == Element.Dark),
                _ => player.Bag.Where(x => x.Type == ItemType.道具),
            };
            foreach (ItemEquip i in items.Where(x => !x.IsEquiped && x.Id != Guid.Empty))
            {
                _ = cb.WithButton(i.Name, "sell-" + i.Id.ToString());
            }
            return message.RespondAsync("你打開左背包發現有...", components: cb.Build(), ephemeral: true);
        }
    }
}
