using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;

namespace MyaDiscordBot.Commands
{
    public class Equip : ICommand
    {
        public string Name => "equip";

        public string Description => "Equip your items to be used in next battle";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            ComponentBuilder builder = new();
            _ = builder.WithButton("火", "equipType-fire");
            _ = builder.WithButton("風", "equipType-wind");
            _ = builder.WithButton("土", "equipType-earth");
            _ = builder.WithButton("水", "equipType-water");
            _ = builder.WithButton("光", "equipType-light");
            _ = builder.WithButton("暗", "equipType-dark");
            _ = builder.WithButton("神", "equipType-god");
            _ = builder.WithButton("道具", "equipType-items");
            return command.RespondAsync("選擇背包內道具種類：", ephemeral: true, components: builder.Build());
        }
    }
}
