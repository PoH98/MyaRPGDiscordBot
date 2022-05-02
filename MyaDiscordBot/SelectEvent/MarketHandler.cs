using Discord.WebSocket;
using MyaDiscordBot.GameLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyaDiscordBot.SelectEvent
{
    public class MarketHandler : ISelectHandler
    {
        private readonly IMarketService marketService;
        private readonly IPlayerService playerService;
        public MarketHandler(IMarketService marketService, IPlayerService playerService)
        {
            this.marketService = marketService;
            this.playerService = playerService;
        }
        public bool CheckUsage(string command)
        {
            return command.StartsWith("market-");
        }

        public async Task Handle(SocketMessageComponent message, DiscordSocketClient client)
        {
            var player = playerService.LoadPlayer(message.User.Id, (message.Channel as SocketGuildChannel).Guild.Id);
            try
            {
                var resource = await marketService.Purchase(player, message.Data.Values.First().ToString(), client);
                if (resource != null)
                {
                    await message.RespondAsync("購買成功！", ephemeral: true);
                    playerService.SavePlayer(player);
                }
                else
                {
                    await message.RespondAsync("你冇錢買依樣野！", ephemeral: true);
                }
            }
            catch(ArgumentException)
            {
                await message.RespondAsync("你對著空氣買野，其他市集裡的人都覺得你奇奇怪怪", ephemeral: true);
            }
            catch(Exception ex)
            {
                await message.RespondAsync(ex.ToString());
            }
        }
    }
}
