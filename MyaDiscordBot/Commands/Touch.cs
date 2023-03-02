using Discord;
using Discord.WebSocket;
using MyaDiscordBot.Commands.Base;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Touch : ICommand
    {
        private readonly IPlayerService _playerService;
        public Touch(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        public string Name => "touch";

        public string Description => "Touch Mya";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            if (DateTime.Now.Hour is < 6 and > 0)
            {
                return command.RespondAsync("zzZZZ (米亞已經訓著，無法回復你哦！)", ephemeral: true);
            }
            Random rnd = new();
            if (rnd.NextDouble() < 0.05)
            {
                Models.Player player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
                player.CurrentHP = 1;
                _playerService.SavePlayer(player);
                return command.RespondAsync("米亞發火用電擊棒治療左" + command.User.Mention + "一餐！");
            }

            return command.RespondAsync(command.User.Mention + "啊~唔好掂我呀！米亞大叫一聲之後整個樓層的玩家都聽到嗮");
        }
    }
}
