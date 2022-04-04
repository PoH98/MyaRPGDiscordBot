using Discord;
using Discord.WebSocket;

namespace MyaDiscordBot.Commands
{
    public class Shop : ICommand
    {
        public string Name => "shop";

        public string Description => "Shop purchase";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command)
        {

            await command.RespondAsync("小貓精靈受到你的召喚，已經出現係你面前！≧◉ᴥ◉≦\n商品列表：");
        }
    }
}
