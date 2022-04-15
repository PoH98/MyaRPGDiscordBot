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
            try
            {
                setting.SetChannel(command);
                return command.RespondAsync("Done", ephemeral: true);
            }
            catch
            {
                return command.RespondAsync("你無權限綁定Channel作為遊戲用途！", ephemeral: true);
            }

        }
    }
}
