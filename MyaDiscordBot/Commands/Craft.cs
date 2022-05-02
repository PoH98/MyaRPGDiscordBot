using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Craft : ICommand
    {
        private readonly IItemService itemService;
        private readonly IPlayerService playerService;
        public Craft(IItemService itemService, IPlayerService playerService)
        {
            this.itemService = itemService;
            this.playerService = playerService;

        }
        public string Name => "craft";

        public string Description => "Craft Items";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            var items = itemService.GetCraftItem();
            var cb = new ComponentBuilder();
            foreach (var i in items.Where(i => !player.Bag.Any(y => i.Id == y.Id)))
            {
                cb.WithButton(i.Name, "craft-" + i.Id);
            }
            return command.RespondAsync("可合成列表：", components: cb.Build(), ephemeral: true);
        }
    }
}
