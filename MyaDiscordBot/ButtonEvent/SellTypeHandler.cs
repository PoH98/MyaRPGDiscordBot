using Discord;
using Discord.WebSocket;
using MyaDiscordBot.ButtonEvent.Base;
using MyaDiscordBot.GameLogic.Services;
using MyaDiscordBot.Models;
using System.Text;

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
            StringBuilder sb = new();
            sb.Append("你打開左背包發現有...");
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
                try
                {
                    _ = cb.WithButton(i.Name, "sell-" + i.Id.ToString());
                }
                catch (ArgumentException ex)
                {
                    //less than 5
                    if (ex.Message.Contains('5'))
                    {
                        sb.Append("（裝備數量太多，無法顯示所有裝備！）");
                        break;
                    }
                }
            }
            return message.RespondAsync(sb.ToString(), components: cb.Build(), ephemeral: true);
        }
    }
}
