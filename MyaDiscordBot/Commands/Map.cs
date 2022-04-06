using Discord;
using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.Commands
{

    public class Map : ICommand
    {
        private readonly IMapService _mapService;
        private readonly IPlayerService _playerService;
        public Map(IMapService mapService, IPlayerService player)
        {
            _mapService = mapService;
            _playerService = player;
        }
        public string Name => "map";

        public string Description => "Open your map";

        public IEnumerable<SlashCommandOptionBuilder> Option => new SlashCommandOptionBuilder[0];

        public async Task Handler(SocketSlashCommand command, DiscordSocketClient client)
        {
            var player = _playerService.LoadPlayer(command.User.Id, (command.Channel as SocketGuildChannel).Guild.Id);
            using (var stream = _mapService.GetMap(player.Coordinate, _mapService.GetCurrentMap((command.Channel as SocketGuildChannel).Guild.Id)))
            {
                await command.RespondWithFileAsync(stream, "map.jpg", "黃色圈 = 你\n紫色方塊 = 玩家重生點", ephemeral: true);
            }
        }
    }
}
