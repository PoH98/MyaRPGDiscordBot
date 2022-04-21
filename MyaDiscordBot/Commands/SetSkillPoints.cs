using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class SetSkillPoints : ICommand
    {
        private readonly IPlayerService playerService;
        public SetSkillPoints(IPlayerService playerService)
        {
            this.playerService = playerService;
        }
        public string Name => "point";

        public string Description => "Check and use your LV up points";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id, command.User.Username);
            if (player.SkillPoint < 1)
            {
                return command.RespondAsync("你擁有的點數：" + player.SkillPoint, ephemeral: true);
            }
            ComponentBuilder cb = new ComponentBuilder();
            cb.WithButton("攻擊力", "skill-atk");
            cb.WithButton("防御力", "skill-def");
            cb.WithButton("生命值", "skill-hp");
            return command.RespondAsync("你擁有的點數：" + player.SkillPoint + "\n可將點數放落以下數值中：", components: cb.Build(), ephemeral: true);
        }
    }
}
