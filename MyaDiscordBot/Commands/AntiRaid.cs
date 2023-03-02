using Autofac;
using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    internal class AntiRaid : ICommand
    {
        public string Name => "raidclear";

        public string Description => "Start or cancel Anti-Raid system";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            SocketGuildUser user = command.User as SocketGuildUser;
            if (command.User.Id == 294835963442757632 || (!command.User.IsBot && (user.GuildPermissions.BanMembers || user.GuildPermissions.KickMembers || user.GuildPermissions.Administrator || user.GuildPermissions.ManageGuild)))
            {
                using ILifetimeScope scope = Data.Instance.Container.BeginLifetimeScope();
                ISettingService setting = scope.Resolve<ISettingService>();
                Models.ServerSettings s = setting.GetSettings((command.Channel as SocketGuildChannel).Guild.Id);
                s.RaidKiller = !s.RaidKiller;
                setting.SaveSettings((command.Channel as SocketGuildChannel).Guild.Id, s);
                _ = command.RespondAsync("已經切換成功！當前狀態：" + (s.RaidKiller ? "已經開啟" : "已經關閉"), ephemeral: true);
            }
            else
            {
                _ = command.RespondAsync("你冇權限使用此指令！", ephemeral: true);
            }
            return Task.CompletedTask;
        }
    }
}
