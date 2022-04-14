using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Setting : ICommand
    {
        private readonly ISettingService setting;
        public Setting(ISettingService setting)
        {
            this.setting = setting;
        }
        public string Name => "setting";

        public string Description => "Define which channel to be used for this bot, only Admin can use";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            setting.SetChannel(command);
            return Task.CompletedTask;
        }
    }
}
