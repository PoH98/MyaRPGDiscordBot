using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Craft : ICommand
    {
        private readonly IItemService itemService;
        public Craft(IItemService itemService)
        {
            this.itemService = itemService;

        }
        public string Name => "craft";

        public string Description => "Craft Items";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var items = itemService.GetCraftItem();
            var cb = new ComponentBuilder();
            foreach (var i in items)
            {
                cb.WithButton(i.Name, "craft-" + i.Id);
            }
            return command.RespondAsync("可合成列表：", components: cb.Build(), ephemeral: true);
        }
    }
}
