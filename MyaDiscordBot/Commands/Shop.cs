using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Shop : ICommand
    {
        private readonly IItemService itemService;
        private readonly IPlayerService playerService;
        public Shop(IItemService itemService, IPlayerService playerService)
        {
            this.itemService = itemService;
            this.playerService = playerService;
        }
        public string Name => "shop";

        public string Description => "Shop purchase";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            var items = itemService.GetShopItem(player);
            var builder = new ComponentBuilder();
            foreach (var i in items)
            {
                //Already have this thing
                if(player.Bag.Any(x => x.Name == i.Name) && i.UseTimes == -1)
                {
                    builder.WithButton(i.Name + " - " + i.Price + "金幣 (你已擁有此道具)", "shop-" + i.Id.ToString(), disabled: true);
                }
                else
                {
                    builder.WithButton(i.Name + " - " + i.Price + "金幣", "shop-" + i.Id.ToString());
                }
            }
            await command.RespondAsync("小貓精靈受到你的召喚，已經出現係你面前！≧◉ᴥ◉≦\n商品列表：", components: builder.Build(), ephemeral: true);
        }
    }
}
