using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;

namespace MyaDiscordBot.SelectEvent
{
    public class MarketSellCountPriceHandler : ISelectHandler
    {
        private readonly IMarketService marketService;
        private readonly IPlayerService playerService;
        public MarketSellCountPriceHandler(IMarketService marketService, IPlayerService playerService)
        {
            this.playerService = playerService;
            this.marketService = marketService;
        }
        public bool CheckUsage(string command)
        {
            return command.StartsWith("mrkSCPrice-");
        }

        public Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var split = message.Data.Values.First().Split("-");
            var amount = Convert.ToInt32(split[1]);
            var id = string.Join("-", split[2], split[3], split[4], split[5], split[6]);
            var price = Convert.ToInt32(split[7]) * amount;
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            var sellid = marketService.Sell(player, id, amount, price);
            if (sellid != Guid.Empty)
            {
                playerService.SavePlayer(player);
                return message.RespondAsync("你的出售訂單ID為" + sellid + "。上架成功！當前價格：" + price + "$", ephemeral: true);
            }
            else
            {
                return message.RespondAsync("你成功出售左空氣，獲得0$！", ephemeral: true);
            }
        }
    }
}
