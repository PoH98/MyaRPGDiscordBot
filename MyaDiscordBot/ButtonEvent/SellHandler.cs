using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.ButtonEvent
{
    public class SellHandler : IButtonHandler
    {
        private IPlayerService _playerService;
        public SellHandler(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            if (command.StartsWith("sell-"))
            {
                return true;
            }
            return false;
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var player = _playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id, message.User.Username);
            var item = player.Bag.FirstOrDefault(x => x.Id.ToString() == message.Data.CustomId.Replace("sell-", ""));
            if (item == null)
            {
                await message.RespondAsync("你卑甘米一堆空氣，甘米對著你關上左當鋪大門！", ephemeral: true);
                return;
            }
            player.Bag.Remove(item);
            player.Coin += 3;
            await message.RespondAsync("你賣左" + item.Name + "，獲得3蚊！", ephemeral: true);
            _playerService.SavePlayer(player);
        }
    }
}
