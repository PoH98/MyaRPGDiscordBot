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
            using LiteDatabase db = new("Filename=save\\" + serverId + ".db;connection=shared");
            ILiteCollection<MarketData> col = db.GetCollection<MarketData>("market");
            return col.FindAll();
        }

        public async Task<Resource> Purchase(Player player, string sellId, DiscordSocketClient client)
        {
            using LiteDatabase db = new("Filename=save\\" + player.ServerId + ".db;connection=shared");
            ILiteCollection<MarketData> col = db.GetCollection<MarketData>("market");
            MarketData market = col.FindOne(x => x.Id == Guid.Parse(sellId.Replace("market-", "")));
            if (market == null)
            {
                throw new ArgumentException();
            }
            if (player.Coin <= market.Price && player.Id != market.PlayerId)
            {
                return null;
            }
            if (player.Id != market.PlayerId)
            {
                player.Coin -= market.Price;
                Player seller = _playerService.LoadPlayer(market.PlayerId, player.ServerId);
                seller.Coin += market.Price - (market.Price * 3 / 100);
                IUser user = client.GetUser(market.DiscordSellerId);
                user ??= (RestUser)await client.GetUserAsync(market.DiscordSellerId);
                try
                {
                    IDMChannel dm = await user.CreateDMChannelAsync();
                    _ = await dm.SendMessageAsync("你已上架的出售ID " + market.Id + "已經出售！");
                    //try DM
                }
                catch
                {

                }

                _playerService.SavePlayer(seller);
            }
            else
            {
                IDMChannel dm = await client.GetUser(market.DiscordSellerId).CreateDMChannelAsync();
                _ = await dm.SendMessageAsync("你已上架的出售ID " + market.Id + "已經下架！");
            }
            Resource resource = _resource.First(x => x.Id.ToString() == market.ResourceId);
            for (int x = 0; x < market.Amount; x++)
            {
                _ = _playerService.AddResource(player, resource);
            }
            _ = col.Delete(market.Id);
            return resource;
        }

        public Guid Sell(Player player, string id, int amount, int price)
        {
            Resource res = _resource.FirstOrDefault(x => x.Id.ToString() == id);
            if (!player.ResourceBag.Any(x => x.Id == res.Id && x.Amount >= amount))
            {
                return Guid.Empty;
            }
            Guid sellId = Guid.NewGuid();
            using (LiteDatabase db = new("Filename=save\\" + player.ServerId + ".db;connection=shared"))
            {
                ILiteCollection<MarketData> col = db.GetCollection<MarketData>("market");
                _ = col.Insert(new MarketData
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
