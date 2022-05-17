using Discord;
using Discord.Rest;
using Discord.WebSocket;
using LiteDB;
using MyaDiscordBot.Models;

namespace MyaDiscordBot.GameLogic.Services
{
    public interface IMarketService
    {
        Guid Sell(Player player, string id, int amount, int price);
        Task<Resource> Purchase(Player player, string sellId, DiscordSocketClient client);
        IEnumerable<MarketData> GetMarketData(ulong serverId);
    }
    public class MarketService : IMarketService
    {
        private readonly IPlayerService _playerService;
        private readonly Resources _resource;
        public MarketService(IPlayerService player, Resources resources)
        {
            _resource = resources;
            _playerService = player;
        }

        public IEnumerable<MarketData> GetMarketData(ulong serverId)
        {
            using (var db = new LiteDatabase("Filename=save\\" + serverId + ".db;connection=shared"))
            {
                var col = db.GetCollection<MarketData>("market");
                return col.FindAll();
            }
        }

        public async Task<Resource> Purchase(Player player, string sellId, DiscordSocketClient client)
        {
            using (var db = new LiteDatabase("Filename=save\\" + player.ServerId + ".db;connection=shared"))
            {
                var col = db.GetCollection<MarketData>("market");
                var market = col.FindOne(x => x.Id == Guid.Parse(sellId.Replace("market-","")));
                if(market == null)
                {
                    throw new ArgumentException();
                }
                if(player.Coin <= market.Price && player.Id != market.PlayerId)
                {
                    return null;
                }
                if(player.Id != market.PlayerId)
                {
                    player.Coin -= market.Price;
                    var seller = _playerService.LoadPlayer(market.PlayerId, player.ServerId);
                    seller.Coin += (market.Price - (market.Price * 3 / 100));
                    IUser user = client.GetUser(market.DiscordSellerId);
                    if(user == null)
                    {
                        user = (RestUser)await client.GetUserAsync(market.DiscordSellerId);
                    }
                    var dm = await user.CreateDMChannelAsync();
                    await dm.SendMessageAsync("你已上架的出售ID " + market.Id + "已經出售！");
                    _playerService.SavePlayer(seller);
                }
                else
                {
                    var dm = await client.GetUser(market.DiscordSellerId).CreateDMChannelAsync();
                    await dm.SendMessageAsync("你已上架的出售ID " + market.Id + "已經下架！");
                }
                var resource = _resource.First(x => x.Id.ToString() == market.ResourceId);
                for(int x = 0; x < market.Amount; x++)
                {
                    _playerService.AddResource(player, resource);
                }
                col.Delete(market.Id);
                return resource;
            }
        }

        public Guid Sell(Player player, string id, int amount, int price)
        {
            var res = _resource.FirstOrDefault(x => x.Id.ToString() == id);
            if(!player.ResourceBag.Any(x => x.Id == res.Id && x.Amount >= amount))
            {
                return Guid.Empty;
            }
            var sellId = Guid.NewGuid();
            using (var db = new LiteDatabase("Filename=save\\" + player.ServerId + ".db;connection=shared"))
            {
                var col = db.GetCollection<MarketData>("market");
                col.Insert(new MarketData
                {
                    Id = sellId,
                    PlayerId = player.Id,
                    DiscordSellerId = player.DiscordId,
                    ResourceId = id,
                    Amount = amount,
                    Price = price
                });
            }
            player.ResourceBag.First(x => x.Id == res.Id).Amount -= amount;
            return sellId;
        }
    }
}
