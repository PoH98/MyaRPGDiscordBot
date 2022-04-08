using Discord;
using Discord.WebSocket;

namespace MyaDiscordBot.Commands
{
    public class Touch : ICommand
    {
        public string Name => "touch";

        public string Description => "Touch Mya";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            return command.RespondAsync(command.User.Mention + "啊~唔好掂我呀！米亞大叫一聲之後整個樓層的玩家都聽到嗮");
        }
    }
}
