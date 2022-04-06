using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{
    public class Search : ICommand
    {
        private readonly IPlayerService _playerService;
        private readonly IBattleService _battleService;
        public Search(IPlayerService playerService, IBattleService battleService)
        {
            _playerService = playerService;
            _battleService = battleService;
        }
        public string Name => "search";

        public string Description => "Search around in your current location";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            if (player.NextCommand > DateTime.Now)
            {
                await command.RespondAsync("你正在休息！無法進行任何探索或者戰鬥！", ephemeral: true);
                return;
            }
            var enemy = _playerService.Walk(player, 5);//stay
            if (enemy != null)
            {
                var br = _battleService.Battle(enemy, player);
                if (br.IsVictory)
                {
                    player.Coin += 2;
                    player.Exp += 1;
                    await command.RespondAsync("Player Coordinate now " + player.Coordinate.X + "," + player.Coordinate.Y + " and met enemy " + enemy.Name + " and Dealt " + br.DamageDealt + "dmg, Received " + br.DamageReceived + "dmg");
                }
                else
                {
                    await command.RespondWithFileAsync("Assets\\wasted.png", "你已死亡，請等待醫護熊貓搬你返基地！復活時間：<t:" + ((DateTimeOffset)player.NextCommand.ToUniversalTime()).ToUnixTimeSeconds() + ":R>");
                }
            }
            else
            {
                await command.RespondAsync("Player Coordinate now " + player.Coordinate.X + "," + player.Coordinate.Y + " and no enemy met, suppose went random event");
            }
            _playerService.SavePlayer(player);
        }
    }
}
