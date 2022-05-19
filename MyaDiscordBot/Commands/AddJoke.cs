using Discord;
using Discord.WebSocket;

namespace MyaDiscordBot.Commands
{
    public class AddJoke : ICommand
    {
        public string Name => "addjoke";

        public string Description => "Tell Mya more new jokes!";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[1] { new SlashCommandOptionBuilder().WithType(ApplicationCommandOptionType.String).WithName("joke").WithDescription("The joke content").WithRequired(true) };

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            if (!File.Exists("joke.txt"))
            {
                File.WriteAllText("joke.txt", "");
            }
            await File.AppendAllTextAsync("joke.txt", ((string)command.Data.Options.First().Value).Replace("\n", "\\n") + "\n");
            if (DateTime.Now.Hour < 6 && DateTime.Now.Hour > 0)
            {
                await command.RespondAsync("zzZZZ (米亞已經訓著，無法回復你哦！不過小熊貓醫護隊已經偷偷幫你記錄底啦！)", ephemeral: true);
                return;
            }
            await command.RespondAsync("米亞非常高興你卑佢知道有咁個好笑野發生！", ephemeral: true);
        }
    }
}
