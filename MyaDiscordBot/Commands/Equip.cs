using Discord;
using Discord.WebSocket;

namespace MyaDiscordBot.Commands
{
    public class Equip : ICommand
    {
        public string Name => "equip";

        public string Description => "Equip your items to be used in next battle";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var builder = new ComponentBuilder();
            builder.WithButton("火", "equipType-fire");
            builder.WithButton("風", "equipType-wind");
            builder.WithButton("土", "equipType-earth");
            builder.WithButton("水", "equipType-water");
            builder.WithButton("光", "equipType-light");
            builder.WithButton("暗", "equipType-dark");
            builder.WithButton("神", "equipType-god");
            builder.WithButton("道具", "equipType-items");
            return command.RespondAsync("選擇背包內道具種類：", ephemeral: true, components: builder.Build());
        }
    }
}
